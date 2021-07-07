using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModSpinTiles : AutoImplementedMod {
		public override string Name => "Spin Tiles";
		public override string Acronym => "SPT";
		public override string Description => "Prepare for a spin";

		public override double ScoreMultiplier => 1;

		public override IconUsage? Icon => FontAwesome.Solid.Spinner;

		public override ModType Type => ModType.Conversion;

		public override bool HasImplementation => true;

		[Modifies( typeof( HitokoriBeatmapConverter ), nameof( HitokoriBeatmapConverter.GenerateSpins ) )]
		private bool GenerateSpins => true;
	}
}
