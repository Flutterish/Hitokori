using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Hitokori.Scoring;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori {
	public class HitokoriRuleset : Ruleset {
		public const string SHORT_NAME = "hitokoriv2";
		public override string Description => SHORT_NAME;
		public override string ShortName => SHORT_NAME;
		public override string PlayingVerb => "Playing with fire";

		public HitokoriRuleset () {
			
		}

		public override IEnumerable<Mod> GetModsFor ( ModType type ) {
			return Array.Empty<Mod>();
		}

		public override DrawableRuleset CreateDrawableRulesetWith ( IBeatmap beatmap, IReadOnlyList<Mod> mods = null )
			=> new DrawableHitokoriRuleset( this, beatmap, mods );

		public override IBeatmapConverter CreateBeatmapConverter ( IBeatmap beatmap )
			=> new HitokoriBeatmapConverter( beatmap, this );
		public override IBeatmapProcessor CreateBeatmapProcessor ( IBeatmap beatmap )
			=> new HitokoriBeatmapProcessor( beatmap );

		public override DifficultyCalculator CreateDifficultyCalculator ( WorkingBeatmap beatmap )
			=> new HitokoriDifficultyCalculator( this, beatmap );

		public override IEnumerable<KeyBinding> GetDefaultKeyBindings ( int variant = 0 ) {
			return new KeyBinding[] {
				new( new KeyCombination( InputKey.Z, InputKey.MouseLeft ), HitokoriAction.Action1 ),
				new( new KeyCombination( InputKey.X, InputKey.MouseRight ), HitokoriAction.Action2 )
			};
		}

		public override string GetDisplayNameForHitResult ( HitResult result ) {
			return result switch {
				HitResult.Perfect => "Perfect",
				HitResult.Ok => "Ok",
				HitResult.Meh => "Meh",
				HitResult.Miss => "Miss",
				_ => $"Invalid {nameof(HitResult)}"
			};
		}

		protected override IEnumerable<HitResult> GetValidHitResults () {
			return new HitResult[] {
				HitResult.Perfect,
				HitResult.Ok,
				HitResult.Meh,
				HitResult.Miss
			};
		}
	}
}
