using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Scoring {
	public class HitokoriJudgement : Judgement {
		protected override int NumericResultFor ( HitResult result ) {
			return result switch
			{
				HitResult.Perfect => 300,
				HitResult.Good => 100,
				HitResult.Meh => 50,

				_ => 0
			};
		}

		protected override double HealthIncreaseFor ( HitResult result )
			=> base.HealthIncreaseFor( result ) * 2;
	}
}
