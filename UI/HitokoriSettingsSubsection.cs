using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hitokori.Settings;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class HitokoriSettingsSubsection : RulesetSettingsSubsection {
		public HitokoriSettingsSubsection ( Ruleset ruleset ) : base( ruleset ) { }

		protected override string Header => HitokoriRuleset.SHORT_NAME;

		[BackgroundDependencyLoader]
		private void load () {
			var config = Config as HitokoriSettingsManager;

			Children = new Drawable[] {
				new SettingsCheckboxWithTooltip {
					LabelText = "Use \"A Dance of Fire and Ice\" style judgement text (in-game only)",
					Bindable = config.GetBindable<bool>( HitokoriSetting.ADOFAIJudgement ),

					TooltipText = "Display \"Early\" and \"Late\" instead of \"Good\", \"Okay\" and \"Meh\""
				},
				new SettingsEnumDropdown<MissCorrectionMode> {
					LabelText = "Miss correction mode",
					Bindable = config.GetBindable<MissCorrectionMode>( HitokoriSetting.MissCorrectionMode )
				},
				new SettingsSlider<double,SpeedSlider> {
					LabelText = "Camera speed",
					Bindable = config.GetBindable<double>( HitokoriSetting.CameraSpeed )
				},
				new SettingsEnumDropdown<CameraFollowMode> {
					LabelText = "Camera follow mode",
					Bindable = config.GetBindable<CameraFollowMode>( HitokoriSetting.CameraFollowMode )
				},
				new SettingsSlider<double,PercentageSlider> {
					LabelText = "Orbit ring opacity",
					Bindable = config.GetBindable<double>( HitokoriSetting.RingOpacity )
				},
				new SettingsEnumDropdown<DashStyle> {
					LabelText = "Orbit ring border style",
					Bindable = config.GetBindable<DashStyle>( HitokoriSetting.RingDashStyle )
				},
				new SettingsSlider<double,PercentageSlider> {
					LabelText = "Connector width",
					Bindable = config.GetBindable<double>( HitokoriSetting.ConnectorWidth )
				},
				new SettingsSlider<double,PercentageSlider> {
					LabelText = "Hold/Spin connector width",
					Bindable = config.GetBindable<double>( HitokoriSetting.HoldConnectorWidth )
				},
			};
		}
	}

	public class SettingsCheckboxWithTooltip : SettingsCheckbox, IHasTooltip {
		public string TooltipText { get; set; }
	}
	public class SpeedSlider : OsuSliderBar<double> {
		public override string TooltipText => $"{Current.Value}ms";
	}
	public class PercentageSlider : OsuSliderBar<double> {
		public override string TooltipText => $"{(int)(Current.Value*100)}%";
	}
}
