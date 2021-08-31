using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Hitokori.UI.Visuals;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawableSwapTilePoint : DrawableHitokoriHitObject<SwapTilePoint> {
		[Resolved]
		private HitokoriPlayfield playfield { get; set; }

		private TilePointVisual visual;
		public DrawableSwapTilePoint () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AddInternal( visual = new() );
		}

		protected override void OnApply () {
			base.OnApply();
			visual.AppliedHitObject = HitObject;
		}

		protected override void OnFree () {
			base.OnFree();
			visual.AppliedHitObject = null;
		}

		protected override void Update () {
			base.Update();

			Position = (Vector2)HitObject.Position * 100;
		}

		protected override void UpdateInitialTransforms () {
			base.UpdateInitialTransforms();
			visual.UpdateInitialTransforms();
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			base.UpdateHitStateTransforms( state );
			if ( state == ArmedState.Hit ) {
				using ( BeginAbsoluteSequence( HitObject.Next?.StartTime ?? HitObject.StartTime ) ) {
					Expire();
				}
			}
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( timeOffset >= 0 ) ApplyResult( j => j.Type = HitResult.Perfect );
		}

		protected override double InitialLifetimeOffset => 2000;
	}
}
