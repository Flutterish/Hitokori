using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Scoring;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Difficulty {
	public class HitokoriPerformanceCalculator : PerformanceCalculator {
		public HitokoriPerformanceCalculator ( Ruleset ruleset, WorkingBeatmap beatmap, ScoreInfo score ) : base( ruleset, beatmap, score ) {

		}

		public override double Calculate ( Dictionary<string, double> categoryDifficulty = null ) {
			throw new NotImplementedException();
		}
	}
}
