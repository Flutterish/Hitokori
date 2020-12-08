using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableTilePoint : DrawableHitokoriHitObject {
		public TilePoint TilePoint;
		public TileMarker Marker;

		public DrawableTilePoint ( HitokoriHitObject hitObject ) : base( hitObject ) {
			TilePoint = hitObject as TilePoint;

			AddInternal(
				Marker = new TileMarker( TilePoint ).Center()
			);

			this.Center();

			if ( TilePoint.ChangedDirection ) {
				Marker.Reverse( TilePoint.IsClockwise );
			}

			OnNewResult += ( x, y ) => OnHit();
			OnRevertResult += ( x, y ) => OnRevert();
		}

		[BackgroundDependencyLoader( true )]
		private void load ( HitokoriSettingsManager config ) {
			if ( TilePoint.IsDifferentSpeed ) {
				if ( config?.Get<bool>( HitokoriSetting.ShowSpeeedChange ) ?? true ) {
					Marker.AddLabel( $"{TilePoint.SpeedDifferencePercent:+####%;-####%}" );
				}// TODO dynamic text
			}
		}

		protected override void UpdateInitialTransforms () {
			Marker.Appear();
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			if ( state == ArmedState.Miss ) {
				LifetimeEnd = TilePoint.HitTime + Marker.Miss();
			}
			else {
				LifetimeEnd = TilePoint.HitTime + Marker.Hit();
			}
		}

		public void SetResult ( HitResult result ) {
			if ( result != HitResult.None ) {
				ApplyResult( j => {
					j.Type = result;
				} );
			}
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( wasReverted && Clock.CurrentTime > revertedTimestamp )
				TryToHitAt( revertedTimestamp ); // HACK this fixes https://github.com/ppy/osu/issues/10811

			// to make sure a result is set
			if ( !TilePoint.CanBeHitAfter( TilePoint.TimeAtOffset( timeOffset ) ) || timeOffset > TilePoint.Duration / 2 ) {
				SetResult( HitResult.Miss ); // NOTE when rewinding this sets off on first tile, at an offset from its actual hit time
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
					if ( attempts > 0 ) return false;
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
				Hitokori.AnimateDistance( duration: TilePoint.Duration, distance: DrawableTapTile.SPACING * ( TilePoint.Next?.Distance ?? 1 ), easing: Easing.None );
			}
			else {
				Hitokori.Swap( TilePoint );
			}
		}
	}
}
