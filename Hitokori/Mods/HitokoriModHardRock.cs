using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModHardRock : AutoImplementedMod {
		public override double ScoreMultiplier => 1.1;
		public override string Name => "Hard Rock";
		public override string Acronym => "HR";
		public override string Description => "Through the fire and the flames";

		public override Type[] IncompatibleMods => base.IncompatibleMods.Concat( new[] { typeof( ModEasy ), typeof( ModDifficultyAdjust ) } ).ToArray();

		public override bool HasImplementation => true;
		public override IconUsage? Icon => OsuIcon.ModHardRock;
		public override ModType Type => ModType.DifficultyIncrease;

		[Modifies( typeof( HitokoriBeatmapConverter ), nameof( HitokoriBeatmapConverter.Speed ) )]
		private double Speed => 3d / 2;

		[Modifies( typeof( HitokoriBeatmapConverter ), nameof( HitokoriBeatmapConverter.Flip ) )]
		private bool Flip => true;
	}
}
