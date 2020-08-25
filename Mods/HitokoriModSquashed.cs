using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;
using System;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModSquashed : Mod, IApplicableToBeatmapConverter {
		public override string Name => "Squashed";
		public override string Acronym => "Squ";
		public override string Description => "Something feels slower";

		public override double ScoreMultiplier => 0.7;

		public override IconUsage? Icon => FontAwesome.Solid.CompressArrowsAlt;

		public override ModType Type => ModType.DifficultyReduction;

		public override bool HasImplementation => true;

		public override Type[] IncompatibleMods => new Type[] { typeof( HitokoriModStretched ), typeof( HitokoriModHardRock ) };

		public void ApplyToBeatmapConverter ( IBeatmapConverter beatmapConverter ) {
			( beatmapConverter as HitokoriBeatmapConverter ).Speed = 0.5;
		}
	}
}
