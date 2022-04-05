using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModAngles : Mod, IApplicableToBeatmapProcessor {
		public HitokoriModAngles () {
			AnglePerBeatPreset.BindValueChanged( v => {
				updateAngles( v.NewValue.GetAngle( AnglePerBeat.Value ) );
			} );

			AnglePerBeat.BindValueChanged( v => {
				updateAngles( v.NewValue );
			} );
		}

		private bool updateLock = false;
		private void updateAngles ( double value ) {
			if ( updateLock ) return;
			updateLock = true;

			AnglePerBeatPreset.Value = value switch {
				180 => PresetAngles.Straight,
				135 => PresetAngles.Octagon,
				120 => PresetAngles.Hexagon,
				108 => PresetAngles.Pentagon,
				90 => PresetAngles.Square,
				_ => PresetAngles.Nonstandard
			};
			AnglePerBeat.Value = value;

			updateLock = false;
		}

		public override string Name => "Adjust Angles";
		public override string SettingDescription => AnglePerBeatPreset.Value is PresetAngles.Nonstandard ? $"{AnglePerBeat.Value}°" : $"[{AnglePerBeatPreset.Value}] {AnglePerBeat.Value}°";
		public override string Acronym => "AA";
		public override string Description => "Look at it from a different angle";
		public override double ScoreMultiplier => 1;
		public override ModType Type => ModType.Conversion;

		public override IconUsage? Icon => OsuIcon.ModSpunOut;
		public override bool HasImplementation => true;

		public override bool RequiresConfiguration => true;

		[SettingSource( "Angle per beat" )]
		public Bindable<double> AnglePerBeat { get; } = new BindableDouble( 180 ) {
			MinValue = 60,
			MaxValue = 270,
			Precision = 1
		};

		[SettingSource( "Path shape (presets)" )]
		public Bindable<PresetAngles> AnglePerBeatPreset { get; } = new( PresetAngles.Straight );

		public void ApplyToBeatmapProcessor ( IBeatmapProcessor beatmapProcessor ) {
			if ( beatmapProcessor is not HitokoriBeatmapProcessor hp ) return;

			hp.ForcedAnglePerBeat = AnglePerBeat.Value;
		}
	}

	public enum PresetAngles {
		Straight = 180,
		Octagon = 135,
		Hexagon = 120,
		Pentagon = 108,
		Square = 90,

		Nonstandard = 0
	}

	public static class PresetAnglesExtensions {
		public static double GetAngle ( this PresetAngles angles, double defaultValue = 180 ) {
			return angles is PresetAngles.Nonstandard ? defaultValue : (double)angles;
		}
	}
}
