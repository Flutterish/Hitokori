using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModEasy : ModEasy {
		public override string Description => HitokoriRuleset.GetLocalisedHack( Localisation.ModStrings.EasyDescription );
	}
}
