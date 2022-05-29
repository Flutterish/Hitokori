using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Scoring {
	public class HitokoriDifficultyCalculator : DifficultyCalculator {
		public HitokoriDifficultyCalculator ( RulesetInfo rulesetInfo, IWorkingBeatmap beatmap ) : base( rulesetInfo, beatmap ) {
		}

		protected override DifficultyAttributes CreateDifficultyAttributes ( IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate ) {
			return new DifficultyAttributes( mods, beatmap.Difficulty.OverallDifficulty );
		}

		protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects ( IBeatmap beatmap, double clockRate ) {
			return Array.Empty<DifficultyHitObject>();
		}

		protected override Skill[] CreateSkills ( IBeatmap beatmap, Mod[] mods, double clockRate ) {
			return Array.Empty<Skill>();
		}
	}
}
