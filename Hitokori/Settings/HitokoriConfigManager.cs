using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.Hitokori.Settings {
	public class HitokoriConfigManager : RulesetConfigManager<HitokoriSetting> {
		public HitokoriConfigManager ( SettingsStore settings, RulesetInfo ruleset, int? variant = null ) : base( settings, ruleset, variant ) { }

		public const float DefaultPositionScale = 90f * 0.8f * 0.6f;

		protected override void InitialiseDefaults () {
			float defaultScale = 90f * 0.8f * 0.6f;
			float minScale = defaultScale * 0.8f;
			float maxScale = defaultScale * 1.8f;

			SetDefault( HitokoriSetting.PositionScale, defaultScale, minScale, maxScale, 1f * 0.6f );
			SetDefault( HitokoriSetting.DoKiaiBeat, true );

			base.InitialiseDefaults();
		}
	}
}
