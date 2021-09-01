using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Hitokori.UI.Visuals;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawablePassThroughTilePoint : DrawableHitokoriHitObject<PassThroughTilePoint, TapPointVisual> {
		public DrawablePassThroughTilePoint () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;
		}

		protected override void OnApply () {
			base.OnApply();
			Position = (Vector2)HitObject.Position * HitokoriPlayfield.PositionScale;
		}

		protected override void Update () {
			base.Update();

			Position = (Vector2)HitObject.Position * HitokoriPlayfield.PositionScale;
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( timeOffset >= 0 ) ApplyResult( j => j.Type = HitResult.Perfect );
		}

		protected override double InitialLifetimeOffset => 2000;
	}
}
