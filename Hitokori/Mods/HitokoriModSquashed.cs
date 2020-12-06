using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModSquashed : AutoImplementedMod {
		public override string Name => "Squashed";
		public override string Acronym => "Squ";
		public override string Description => "Something feels slower";

		public override double ScoreMultiplier => 0.7;

		public override IconUsage? Icon => FontAwesome.Solid.CompressArrowsAlt;

		public override ModType Type => ModType.DifficultyReduction;

		public override bool HasImplementation => true;

		[Modifies( typeof( HitokoriBeatmapConverter ), nameof( HitokoriBeatmapConverter.Speed ) )]
		private double Speed => 2d / 3;
	}
}
