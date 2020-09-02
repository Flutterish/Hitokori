using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using System;

namespace osu.Game.Rulesets.Hitokori.Difficulty.Skills {
	public class Speed : Skill {
		protected override double SkillMultiplier => 0.8;
		protected override double StrainDecayBase => 0.6;

		/// <summary>
		/// The BPM where strain stops increasing
		/// </summary>
		private const double MAX_BPM = 300;

		/// <summary>
		/// The BPM where strain is 1.0
		/// </summary>
		private const double BASE_BPM = 120;

		protected override double StrainValueOf(DifficultyHitObject current) {
			HitokoriDifficultyHitObject hitokoriCurrent = (HitokoriDifficultyHitObject)current;
			double bpm = Math.Min(hitokoriCurrent.BPM, MAX_BPM);

			return Math.Pow(bpm / BASE_BPM, 0.6);
		}
	}
}
