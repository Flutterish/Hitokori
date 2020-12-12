using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableTilePoint : DrawableHitokoriHitObject {
		public TilePoint TilePoint => HitObject as TilePoint;
		public TileMarker Marker;

		public DrawableTilePoint () : base( null ) {
			this.Center();
			AddInternal(
				Marker = new TileMarker().Center()
			);

			OnNewResult += ( x, y ) => OnHit();
			OnRevertResult += ( x, y ) => OnRevert();
		}

		Bindable<bool> showSpeedChange = new( true );
		[BackgroundDependencyLoader( true )]
		private void load ( HitokoriSettingsManager config ) {
			config?.BindWith( HitokoriSetting.ShowSpeeedChange, showSpeedChange );
			Marker.showLabel.BindTo( showSpeedChange );
		}

		protected override void OnApply () {
			base.OnApply();
			attempts = 2;
			Marker.Apply( TilePoint );
			Colour = Colour4.White;
			Position = Vector2.Zero;

			if ( showSpeedChange.Value && TilePoint.IsDifferentSpeed ) {
				Marker.AddLabel( $"{TilePoint.SpeedDifferencePercent:+####%;-####%}" );
			}
			if ( TilePoint.ChangedDirection ) {
				Marker.Reverse( TilePoint.IsClockwise );
			}
		}

		protected override void OnFree () {
			base.OnFree();
			Marker.Free();
			wasReverted = false;
			wasTarget = false;
		}

		bool wasTarget = false;
		protected override void Update () {
			if ( Hitokori.Target == TilePoint.Previous && !wasTarget ) {
				wasTarget = true;
				this.FadeColour( AccentColour.Value, 100 );
				( ParentHitObject as HitokoriTile )?.ChildTargeted( this );
			}
			else if ( Hitokori.Target != TilePoint.Previous && wasTarget ) {
				wasTarget = false;
				this.FadeColour( Colour4.White, 100 );
				( ParentHitObject as HitokoriTile )?.ChildUntargeted( this );
			}
		}

		protected override void UpdateInitialTransforms () {
			Marker.Appear();
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			if ( state == ArmedState.Miss )
				Marker.Miss();
			else if ( state == ArmedState.Hit )
				Marker.Hit();

			LifetimeEnd = TilePoint.HitTime + 1000;
		}

		public void SetResult ( HitResult result ) {
			if ( result != HitResult.None )
				ApplyResult( j => j.Type = result );
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( wasReverted && Clock.CurrentTime > revertedTimestamp )
				TryToHitAt( revertedTimestamp ); // HACK this fixes https://github.com/ppy/osu/issues/10811

			// to make sure a result is set
			if ( !TilePoint.CanBeHitAfter( TilePoint.TimeAtOffset( timeOffset ) ) || timeOffset > TilePoint.Duration / 2 ) {
				SetResult( HitResult.Miss );
			}
		}

		public bool TryToHit ()
			=> TryToHitAt( Clock.CurrentTime );
		public bool TryToHitAtOffset ( double offset )
			=> TryToHitAt( TilePoint.TimeAtOffset( offset ) );
		int attempts = 2;
		public bool TryToHitAt ( double time ) {
			if ( !Judged ) {
				var result = TilePoint.ResultAt( time );
				if ( result == HitResult.None ) {
					result = HitResult.Miss;
				}
				if ( result == HitResult.Miss ) {
					attempts--;
					if ( attempts > 0 ) {
						Playfield.AttemptLost( this );
						return false;
					}
				}
				SetResult( result );
				return true;
			}
			return false;
		}

		private void OnHit () {
			Attach( TilePoint, Hitokori );
		}

		bool wasReverted;
		double revertedTimestamp;
		private void OnRevert () {
			attempts = 2;
			wasReverted = true;
			revertedTimestamp = Clock.CurrentTime;
			Attach( TilePoint.Previous, Hitokori );
		}

		private static void Attach ( TilePoint TilePoint, DrawableHitokori Hitokori ) {
			if ( TilePoint.Parent == TilePoint.Next?.Parent ) {
				Hitokori.RotateTo( TilePoint.OutAngle + Math.PI - TilePoint.Offset, TilePoint.HitTime, TilePoint.HitTime + TilePoint.Duration );
				Hitokori.AnimateDistance( duration: TilePoint.Duration, distance: DrawableTapTile.SPACING * ( TilePoint.Next?.Distance ?? 1 ), easing: Easing.None ); // TODO move this to hitokori

				Hitokori.Target = TilePoint;
			}
			else {
				Hitokori.Swap( TilePoint );
			}
		}
	}
}
