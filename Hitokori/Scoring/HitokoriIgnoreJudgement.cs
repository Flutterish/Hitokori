using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Scoring {
	public class HitokoriIgnoreJudgement : Judgement {
		protected override double HealthIncreaseFor ( HitResult result ) => 0;
		public override HitResult MaxResult => HitResult.IgnoreHit;
	}
}
