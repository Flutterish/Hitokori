using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Hitokori.UI.Visuals;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawablePassThroughTilePoint : DrawableHitokoriHitObject<PassThroughTilePoint> {
		[Resolved]
		private HitokoriPlayfield playfield { get; set; }

		private TapPointVisual visual;
		public DrawablePassThroughTilePoint () {
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

		//protected override void UpdateInitialTransforms () {
		//	this.FadeIn( 200, Easing.Out );
		//}
		//protected override void UpdateHitStateTransforms ( ArmedState state ) {
		//	if ( state == ArmedState.Hit ) {
		//		this.Delay( 2000 ).FadeOut( 100 );
		//	}
		//}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( timeOffset >= 0 ) ApplyResult( j => j.Type = HitResult.Perfect );
		}

		protected override double InitialLifetimeOffset => 2000;
	}
}
