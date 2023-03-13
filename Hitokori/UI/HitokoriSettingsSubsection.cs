using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hitokori.Settings;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.UI {
	public partial class HitokoriSettingsSubsection : RulesetSettingsSubsection {
		public HitokoriSettingsSubsection ( Ruleset ruleset ) : base( ruleset ) { }

		protected override LocalisableString Header => HitokoriRuleset.SHORT_NAME;

		[BackgroundDependencyLoader]
		private void load () {
			var config = Config as HitokoriSettingsManager;

			Children = new Drawable[] {
				new SettingsSlider<double,SpeedSlider> {
					LabelText = Localisation.SettingStrings.CameraSpeed,
					Current = config.GetBindable<double>( HitokoriSetting.CameraSpeed )
				},
				new SettingsSlider<double,PercentageSlider> {
					LabelText = Localisation.SettingStrings.RingOpacity,
					Current = config.GetBindable<double>( HitokoriSetting.RingOpacity )
				},
				new SettingsEnumDropdown<DashStyle> {
					LabelText = Localisation.SettingStrings.RingStyle,
					Current = config.GetBindable<DashStyle>( HitokoriSetting.RingDashStyle )
				},
				new SettingsSlider<double,PercentageSlider> {
					LabelText = Localisation.SettingStrings.ConnectorWidth,
					Current = config.GetBindable<double>( HitokoriSetting.ConnectorWidth )
				},
				new SettingsSlider<double,PercentageSlider> {
					LabelText = Localisation.SettingStrings.ConnectorHoldWidth,
					Current = config.GetBindable<double>( HitokoriSetting.HoldConnectorWidth )
				},
				new SettingsCheckbox {
					LabelText = Localisation.SettingStrings.ShowSpeed,
					Current = config.GetBindable<bool>( HitokoriSetting.ShowSpeeedChange )
				},
				new ColorPicker<OrbitalColorPicker> {
					LabelText = Localisation.SettingStrings.FirstColor,
					Current = config.GetBindable<Color4>( HitokoriSetting.HiColor )
				},
				new ColorPicker<OrbitalColorPicker> {
					LabelText = Localisation.SettingStrings.SecondColor,
					Current = config.GetBindable<Color4>( HitokoriSetting.KoriColor )
				},
				new ColorPicker<OrbitalColorPicker> {
					LabelText = Localisation.SettingStrings.ThirdColor,
					Current = config.GetBindable<Color4>( HitokoriSetting.GreenBitchColor )
				}
			};
		}
	}

	public class SpeedSlider : RoundedSliderBar<double> {
		
		public override LocalisableString TooltipText => $"{Current.Value:N1}x";
    }
	public class PercentageSlider : RoundedSliderBar<double> {
		public override LocalisableString TooltipText => $"{Current.Value:##0%}";
    }
}
