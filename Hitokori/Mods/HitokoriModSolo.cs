using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModSolo : Mod, IApplicableToBeatmapProcessor, IApplicableToBeatmapConverter {
		public override string Name => "Solo";
		public override string Acronym => "SL";
		public override string Description => "Play with just 1 orbital";
		public override double ScoreMultiplier => 1;

		public override IconUsage? Icon => FontAwesome.Solid.Adjust;
		public override Type[] IncompatibleMods => new Type[] { typeof( HitokoriModOrbitals ) };
		public override ModType Type => ModType.Fun;

		public void ApplyToBeatmapProcessor ( IBeatmapProcessor beatmapProcessor ) {
			if ( beatmapProcessor is not HitokoriBeatmapProcessor bp ) return;

			bp.Variant = GameVariant.Solo;
		}

		public void ApplyToBeatmapConverter ( IBeatmapConverter beatmapConverter ) {
			if ( beatmapConverter is not HitokoriBeatmapConverter bc ) return;

			bc.ForcedOrbitalCount = 1;
			bc.Variant = GameVariant.Solo;
		}
	}
}
