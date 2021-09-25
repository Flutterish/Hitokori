using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.Hitokori.Settings {
	public class HitokoriConfigManager : RulesetConfigManager<HitokoriSetting> {
		public HitokoriConfigManager ( SettingsStore settings, RulesetInfo ruleset, int? variant = null ) : base( settings, ruleset, variant ) { }

		protected override void InitialiseDefaults () {
			SetDefault( HitokoriSetting.PositionScale, 90f * 0.6f, 90f * 0.65f * 0.6f, 90f * 1.5f * 0.6f, 1f * 0.6f );
			SetDefault( HitokoriSetting.DoKiaiBeat, true );

			base.InitialiseDefaults();
		}
	}
}
