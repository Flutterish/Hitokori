using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawableNoJudgementTilePoint : DrawableTilePointWithConnections<NoJudgementTilePoint> {
		public DrawableNoJudgementTilePoint () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;
		}

		private BindableFloat positionScale = new( HitokoriPlayfield.DefaultPositionScale );
		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );
		}

		protected override void OnApply () {
			base.OnApply();
			Position = (Vector2)HitObject.Position * positionScale.Value;
		}

		protected override void Update () {
			base.Update();

			Position = (Vector2)HitObject.Position * positionScale.Value;
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( timeOffset >= 0 ) ApplyResult( j => j.Type = HitResult.IgnoreHit );
		}

		protected override double InitialLifetimeOffset => 2000;
	}
}
