using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableTilePoint : DrawableHitokoriHitObject {
		public TilePoint TilePoint;
		public DrawableHitokori Hitokori => ( Parent as HitokoriTile ).Hitokori;

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

		protected override void UpdateInitialTransforms () {
			Marker.Appear();
		}

		protected override void UpdateStateTransforms ( ArmedState state ) {
			switch ( state ) {
				case ArmedState.Idle:
					break;

				case ArmedState.Miss:
					LifetimeEnd = TilePoint.HitTime + Marker.Miss();
					break;

				case ArmedState.Hit:
					LifetimeEnd = TilePoint.HitTime + Marker.Hit();
					break;
			}
		}

		public void SetResult ( HitResult result ) {
			if ( result != HitResult.None ) {
				ApplyResult( j => {
					j.Type = result;
					TilePoint.WasHit = true;
				} );
			}
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			// to make sure a result is set
			if ( !TilePoint.CanBeHitAfter( TilePoint.TimeAtOffset( timeOffset ) ) ) {
				SetResult( HitResult.Miss );
			}
		}

		public bool TryToHit ( double timeOffset )
			=> TryToHitAt( TilePoint.TimeAtOffset( timeOffset ) );
		public bool TryToHit ()
			=> TryToHitAt( Clock.CurrentTime );
		public bool TryToHitAt ( double time ) {
			if ( TilePoint.IsNext ) {
				if ( TilePoint.IgnoresInputAt( time ) ) {
					return false;
				}
				var result = TilePoint.ResultAt( time );
				SetResult( ( result == HitResult.None ) ? HitResult.Miss : result );
				return true;
			}
			return false;
		}

		private void OnHit () {
			TilePoint.WasHit = true;

			Attach( TilePoint, Hitokori );
		}

		private void OnRevert () {
			TilePoint.WasHit = false;

			Attach( TilePoint.Previous, Hitokori );
		}

		private static void Attach ( TilePoint TilePoint, DrawableHitokori Hitokori ) {
			if ( TilePoint.Parent == TilePoint.Next?.Parent ) {
				Hitokori.RotateTo( TilePoint.OutAngle + Math.PI - TilePoint.TripletOffset, TilePoint.HitTime, TilePoint.HitTime + TilePoint.Duration );
				Hitokori.AnimateDistance( duration: TilePoint.Duration, distance: DrawableTapTile.SPACING * ( TilePoint.Next?.Distance ?? 1 ), easing: Easing.None );
			} else {
				Hitokori.Swap( TilePoint );
			}
		}
	}
}
