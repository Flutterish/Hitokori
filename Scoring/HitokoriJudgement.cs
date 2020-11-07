using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Scoring {
	public class HitokoriJudgement : Judgement {
		protected override double HealthIncreaseFor ( HitResult result )
			=> base.HealthIncreaseFor( result ) * 2;
	}
}
