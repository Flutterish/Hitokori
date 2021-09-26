using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Hitokori.UI.Visuals;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawableTapTilePoint : DrawableHitokoriHitObject<PassThroughTilePoint>, IKeyBindingHandler<HitokoriAction> {
		TapPointVisualPiece piece;

		public DrawableTapTilePoint () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AddInternal( piece = new TapPointVisualPiece() );
		}

		protected override void OnApply () {
			base.OnApply();

			if ( HitObject.FromPrevious is IHasVelocity fromv && HitObject.ToNext is IHasVelocity tov ) {
				if ( fromv.Speed / tov.Speed < 0.95 ) {
					piece.Colour = Colour4.Tomato;
				}
				else if ( tov.Speed / fromv.Speed < 0.95 ) {
					piece.Colour = Colour4.LightBlue;
				}
				else {
					piece.Colour = Colour4.HotPink;
				}
			}
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( userTriggered ) {
				var result = HitObject.HitWindows.ResultFor( timeOffset );

				if ( result is HitResult.None ) {
					// TODO some animation to show the "too early"
				}
				else {
					ApplyResult( j => j.Type = result );
				}
			}
			else if ( !HitObject.HitWindows.CanBeHit( timeOffset ) ) {
				ApplyResult( j => j.Type = HitResult.Miss );
			}
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			if ( state is ArmedState.Hit or ArmedState.Miss )
				LifetimeEnd = piece.LatestTransformEndTime;
		}

		public bool OnPressed ( KeyBindingPressEvent<HitokoriAction> action ) {
			if ( Judged ) return false;

			UpdateResult( true );
			return true;
		}

		public void OnReleased ( KeyBindingReleaseEvent<HitokoriAction> action ) { }

		protected override double InitialLifetimeOffset => 2000;
	}
}
