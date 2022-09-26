using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModUntangle : AutoImplementedMod {
		public override string Name => "Untangle";
		public override string Acronym => "UN";
		public override LocalisableString Description => Localisation.ModStrings.UntangleDescription;

		public override double ScoreMultiplier => 1;

		public override IconUsage? Icon => FontAwesome.Solid.Ribbon;
		public override ModType Type => ModType.Conversion;

		[Modifies( typeof( HitokoriBeatmapConverter ), nameof( HitokoriBeatmapConverter.Untangle ) )]
		private bool Untangle => true;
	}
}
