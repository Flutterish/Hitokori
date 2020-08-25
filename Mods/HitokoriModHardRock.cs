using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;
using System;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModHardRock : ModHardRock, IApplicableToBeatmapConverter {
		public override double ScoreMultiplier => 1.1;
		public override string Description => "Through the fire and the flames you go";

		public override bool HasImplementation => true;

		public override Type[] IncompatibleMods => new Type[] { typeof( HitokoriModSquashed ), typeof( HitokoriModStretched ) };

		public void ApplyToBeatmapConverter ( IBeatmapConverter beatmapConverter ) {
			( beatmapConverter as HitokoriBeatmapConverter ).Speed = 1.5;
			( beatmapConverter as HitokoriBeatmapConverter ).Flip = true;
		}
	}
}
