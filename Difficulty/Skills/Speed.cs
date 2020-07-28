using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Difficulty.Skills {
	public class Speed : Skill {
		protected override double SkillMultiplier => 1;

		protected override double StrainDecayBase => 0.3;

		protected override double StrainValueOf ( DifficultyHitObject current ) {
			throw new NotImplementedException();
		}
	}
}
