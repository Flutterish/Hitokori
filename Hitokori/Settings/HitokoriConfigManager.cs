using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.Hitokori.Settings {
	public class HitokoriConfigManager : RulesetConfigManager<HitokoriSetting> {
		public HitokoriConfigManager ( SettingsStore settings, RulesetInfo ruleset, int? variant = null ) : base( settings, ruleset, variant ) { }

		protected override void InitialiseDefaults () {
			SetDefault( HitokoriSetting.PositionScale, 90f, 90f * 0.5f, 90f * 1.5f, 1f );

			base.InitialiseDefaults();
		}
	}
}
