using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Hitokori.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Difficulty {
	public class HitokoriDifficultyCalculator : DifficultyCalculator {
		private const double STAR_SCALING_FACTOR = 0.04125; //Taken from Taiko's difficulty calculations

		public HitokoriDifficultyCalculator ( Ruleset ruleset, WorkingBeatmap beatmap ) : base( ruleset, beatmap ) { }

		protected override DifficultyAttributes CreateDifficultyAttributes ( IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate ) {
			double readingDifficulty = skills[0].DifficultyValue() * STAR_SCALING_FACTOR;
			double speedDifficulty = skills[1].DifficultyValue() * STAR_SCALING_FACTOR;
			double starRating = (readingDifficulty + speedDifficulty) / 2;

			return new DifficultyAttributes {
				StarRating = starRating,
				Mods = mods,
				Skills = skills
			};
		}

		protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects ( IBeatmap beatmap, double clockRate ) {
			for (int i = 1; i < beatmap.HitObjects.Count; i++) {
				HitObject hitObject = beatmap.HitObjects[i];
				TimingControlPoint timingControlPoint = beatmap.ControlPointInfo.TimingPointAt(hitObject.StartTime);

				yield return new HitokoriDifficultyHitObject(hitObject, beatmap.HitObjects[i - 1], timingControlPoint.BPM, clockRate);
			}
		}

		protected override Skill[] CreateSkills ( IBeatmap beatmap ) {
			return new Skill[] { new Reading(), new Speed() };
		}
	}
}
