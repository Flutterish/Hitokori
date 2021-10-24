using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawableNoJudgementTilePoint : DrawableTilePointWithConnections<NoJudgementTilePoint> {
		public DrawableNoJudgementTilePoint () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( timeOffset >= 0 ) ApplyResult( j => j.Type = HitResult.IgnoreHit );
		}
	}
}
