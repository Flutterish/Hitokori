using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModHoldTiles : AutoImplementedMod {
		public override string Name => "Hold Tiles";
		public override string Acronym => "HDT";
		public override string Description => "Adds tiles that you need to hold";

		public override double ScoreMultiplier => 1;

		public override IconUsage? Icon => FontAwesome.Solid.HandHolding;

		public override bool Ranked => false;

		public override ModType Type => ModType.Conversion;

		public override bool HasImplementation => true;

		[Modifies( typeof( HitokoriBeatmapConverter ), nameof( HitokoriBeatmapConverter.NoHolds ) )]
		private bool NoHolds => false;
	}
}
