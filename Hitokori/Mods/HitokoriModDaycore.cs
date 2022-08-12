using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModDaycore : ModDaycore {
		public override string Description => HitokoriRuleset.GetLocalisedHack( Localisation.ModStrings.DaycoreDescription );

		public override double ScoreMultiplier => 0.3;
	}
}
