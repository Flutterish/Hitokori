using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModHoldTiles : AutoImplementedMod {
		public override string Name => "Hold Tiles";
		public override string Acronym => "HDT";
		public override string Description => "Don't hold yourself back";

		public override double ScoreMultiplier => 1;

		public override IconUsage? Icon => FontAwesome.Solid.HandHolding;

		public override ModType Type => ModType.Conversion;

		public override bool HasImplementation => true;

		[Modifies( typeof( HitokoriBeatmapConverter ), nameof( HitokoriBeatmapConverter.NoHolds ) )]
		private bool NoHolds => false;
	}
}
