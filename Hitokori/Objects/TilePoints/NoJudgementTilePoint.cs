using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Objects.TilePoints {
	public class NoJudgementTilePoint : TilePoint {
		public override Judgement CreateJudgement () {
			return new NoJudgement();
		}
	}

	public class NoJudgement : Judgement {
		public override HitResult MaxResult => HitResult.IgnoreHit;
	}
}
