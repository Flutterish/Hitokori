using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori {
	public class HitokoriRuleset : Ruleset {
		public const string SHORT_NAME = "hitokori";
		public override string Description => SHORT_NAME;
		public override string ShortName => SHORT_NAME;
		public override string PlayingVerb => "Playing with fire";

		public HitokoriRuleset () {

		}

		public override IEnumerable<Mod> GetModsFor ( ModType type ) {
			throw new System.NotImplementedException();
		}

		public override DrawableRuleset CreateDrawableRulesetWith ( IBeatmap beatmap, IReadOnlyList<Mod> mods = null ) {
			throw new System.NotImplementedException();
		}

		public override IBeatmapConverter CreateBeatmapConverter ( IBeatmap beatmap ) {
			throw new System.NotImplementedException();
		}

		public override DifficultyCalculator CreateDifficultyCalculator ( WorkingBeatmap beatmap ) {
			throw new System.NotImplementedException();
		}
	}
}
