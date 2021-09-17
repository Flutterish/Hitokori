using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hitokori.Settings;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class HitokoriSettingsSubsection : RulesetSettingsSubsection {
		public HitokoriSettingsSubsection ( Ruleset ruleset ) : base( ruleset ) { }

		protected override LocalisableString Header => HitokoriRuleset.SHORT_NAME;

		[BackgroundDependencyLoader]
		private void load () {
			var config = Config as HitokoriSettingsManager;

			Children = new Drawable[] {
				new SettingsSlider<double,SpeedSlider> {
					LabelText = "Camera speed",
					Current = config.GetBindable<double>( HitokoriSetting.CameraSpeed )
				},
				new SettingsSlider<double,PercentageSlider> {
					LabelText = "Orbit ring opacity",
					Current = config.GetBindable<double>( HitokoriSetting.RingOpacity )
				},
				new SettingsEnumDropdown<DashStyle> {
					LabelText = "Orbit ring border style",
					Current = config.GetBindable<DashStyle>( HitokoriSetting.RingDashStyle )
				},
				new SettingsSlider<double,PercentageSlider> {
					LabelText = "Connector width",
					Current = config.GetBindable<double>( HitokoriSetting.ConnectorWidth )
				},
				new SettingsSlider<double,PercentageSlider> {
					LabelText = "Hold/Spin connector width",
					Current = config.GetBindable<double>( HitokoriSetting.HoldConnectorWidth )
				},
				new SettingsCheckbox {
					LabelText = "Show speed changes below cyan/orange tiles",
					Current = config.GetBindable<bool>( HitokoriSetting.ShowSpeeedChange )
				},
				new ColorPicker<OrbitalColorPicker> {
					LabelText = "First orbital color",
					Current = config.GetBindable<Color4>( HitokoriSetting.HiColor )
				},
				new ColorPicker<OrbitalColorPicker> {
					LabelText = "Second orbital color",
					Current = config.GetBindable<Color4>( HitokoriSetting.KoriColor )
				},
				new ColorPicker<OrbitalColorPicker> {
					LabelText = "Third orbital color",
					Current = config.GetBindable<Color4>( HitokoriSetting.GreenBitchColor )
				},
			};
		}
	}

	public class SpeedSlider : OsuSliderBar<double> {
		public override LocalisableString TooltipText => $"{Current.Value:N1}x";
	}
	public class PercentageSlider : OsuSliderBar<double> {
		public override LocalisableString TooltipText => $"{Current.Value:##0%}";
	}
}
