using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModNoFail : ModNoFail {
		public override string Description => HitokoriRuleset.GetLocalisedHack( Localisation.ModStrings.NoFailDescription );
	}
}
