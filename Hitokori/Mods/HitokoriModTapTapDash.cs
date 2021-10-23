using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;
using System;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModTapTapDash : Mod, IApplicableToBeatmapProcessor, IApplicableToBeatmapConverter {
		public override string Name => "Tap Tap Dash";
		public override string Acronym => "TTD";
		public override string Description => "Play Tap Tap Dash instead";
		public override double ScoreMultiplier => 1;

		public override IconUsage? Icon => FontAwesome.Solid.Crow;
		public override Type[] IncompatibleMods => new Type[] { typeof( HitokoriModOrbitals ) };
		public override ModType Type => ModType.Fun;

		public void ApplyToBeatmapProcessor ( IBeatmapProcessor beatmapProcessor ) {
			if ( beatmapProcessor is not HitokoriBeatmapProcessor bp ) return;

			bp.Variant = GameVariant.TapTapDash;
		}

		public void ApplyToBeatmapConverter ( IBeatmapConverter beatmapConverter ) {
			if ( beatmapConverter is not HitokoriBeatmapConverter bc ) return;

			bc.ForcedOrbitalCount = 1;
		}
	}
}
