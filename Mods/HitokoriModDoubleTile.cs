using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModDoubleTile : AutoImplementedMod {
		public override string Name => "Double Tiles";
		public override string Acronym => "DTs";
		public override string Description => "Double the trouble";

		public override double ScoreMultiplier => 1.420069;

		public override IconUsage? Icon => FontAwesome.Solid.DiceTwo;

		public override bool HasImplementation => true;

		public override ModType Type => ModType.Conversion;

		[Modifies( typeof( HitokoriBeatmapConverter ), nameof( HitokoriBeatmapConverter.DoubleTrouble ) )]
		private bool DoubleTrouble => true;
	}
}
