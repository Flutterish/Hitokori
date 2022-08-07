using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModSuddenDeath : ModSuddenDeath {
		public override string Description => HitokoriRuleset.GetLocalisedHack( Localisation.Mod.Strings.SuddenDeathDescription );
	}
}
