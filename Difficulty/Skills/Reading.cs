using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using System;

namespace osu.Game.Rulesets.Hitokori.Difficulty.Skills {
	public class Reading : Skill {
		protected override double SkillMultiplier => 0.6;

		protected override double StrainDecayBase => 0.6;

		protected override double StrainValueOf ( DifficultyHitObject current ) {
			throw new NotImplementedException();
		}
	}
}
