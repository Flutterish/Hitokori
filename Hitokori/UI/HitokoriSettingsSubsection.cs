using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hitokori.Settings;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class HitokoriSettingsSubsection : RulesetSettingsSubsection {
		protected override LocalisableString Header => HitokoriRuleset.SHORT_NAME;
		public HitokoriSettingsSubsection ( Ruleset ruleset ) : base( ruleset ) { }

		protected override void LoadComplete () {
			var config = Config as HitokoriConfigManager;

			Add( new SettingsSlider<float, ScaleSlider> {
				Current = config.GetBindable<float>( HitokoriSetting.PositionScale ),
				LabelText = "Distance scale"
			} );

			Add( new SettingsCheckbox {
				Current = config.GetBindable<bool>( HitokoriSetting.DoKiaiBeat ),
				LabelText = "Scale screen on kiai beats"
			} );
		}
	}

	public class ScaleSlider : OsuSliderBar<float> {
		public override LocalisableString TooltipText => $"{Current.Value / Current.Default :N2}x";
	}
}
