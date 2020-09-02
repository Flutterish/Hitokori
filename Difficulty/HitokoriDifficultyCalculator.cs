using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Hitokori.Difficulty.Skills;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Difficulty {
	public class HitokoriDifficultyCalculator : DifficultyCalculator {
		private const double STAR_SCALING_FACTOR = 0.04125; //Taken from Taiko's difficulty calculations

		public HitokoriDifficultyCalculator ( Ruleset ruleset, WorkingBeatmap beatmap ) : base( ruleset, beatmap ) { }

		protected override DifficultyAttributes CreateDifficultyAttributes ( IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate ) {
			double readingDifficulty = skills.First<Reading>().DifficultyValue() * STAR_SCALING_FACTOR;
			double speedDifficulty = skills.First<Speed>().DifficultyValue() * STAR_SCALING_FACTOR;
			double starRating = (readingDifficulty + speedDifficulty) / 2;

			return new DifficultyAttributes {
				StarRating = starRating,
				Mods = mods,
				Skills = skills
			};
		}

		protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects ( IBeatmap beatmap, double clockRate ) {
			HitObject last = beatmap.HitObjects.First();
			foreach ( HitObject hitObject in beatmap.HitObjects.Skip( 1 ) ) {
				TimingControlPoint timingControlPoint = beatmap.ControlPointInfo.TimingPointAt( hitObject.StartTime );
				yield return new HitokoriDifficultyHitObject( hitObject, last, timingControlPoint.BPM, clockRate );
				last = hitObject;
			}
		}

		protected override Skill[] CreateSkills ( IBeatmap beatmap ) {
			return new Skill[] { new Reading(), new Speed() };
		}
	}
}
