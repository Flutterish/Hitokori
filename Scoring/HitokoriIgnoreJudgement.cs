using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Scoring {
	public class HitokoriIgnoreJudgement : Judgement {
		public override bool AffectsCombo => false;

		protected override int NumericResultFor ( HitResult result ) => 0;

		protected override double HealthIncreaseFor ( HitResult result ) => 0;
	}
}
