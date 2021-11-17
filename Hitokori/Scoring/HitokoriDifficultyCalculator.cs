using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Scoring {
	public class HitokoriDifficultyCalculator : DifficultyCalculator {
		public HitokoriDifficultyCalculator ( Ruleset ruleset, WorkingBeatmap beatmap ) : base( ruleset, beatmap ) {
		}

		protected override DifficultyAttributes CreateDifficultyAttributes ( IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate ) {
			return new DifficultyAttributes( mods, skills, beatmap.Difficulty.OverallDifficulty );
		}

		protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects ( IBeatmap beatmap, double clockRate ) {
			return Array.Empty<DifficultyHitObject>();
		}

		protected override Skill[] CreateSkills ( IBeatmap beatmap, Mod[] mods, double clockRate ) {
			return Array.Empty<Skill>();
		}
	}
}
