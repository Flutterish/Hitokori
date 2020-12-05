using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.Hitokori.Settings {
	public class HitokoriSettingsManager : RulesetConfigManager<HitokoriSetting> {
		public HitokoriSettingsManager ( SettingsStore settings, RulesetInfo ruleset, int? variant = null ) : base( settings, ruleset, variant ) { }

		protected override void InitialiseDefaults () {
			Set( HitokoriSetting.CameraSpeed, 300, 100, 500, 20.0 );
			Set( HitokoriSetting.RingOpacity, 0.15, 0, 1, 0.01 );
			Set( HitokoriSetting.RingDashStyle, DashStyle.Dashed );
			Set( HitokoriSetting.ConnectorWidth, 1, 0.4, 4 );
			Set( HitokoriSetting.HoldConnectorWidth, 1, 0.2, 2 );
			Set( HitokoriSetting.ShowSpeeedChange, true );

			base.InitialiseDefaults();
		}
	}
}
