﻿using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Hitokori.Settings {
	public enum HitokoriSetting {
		CameraSpeed,
		RingOpacity,
		RingDashStyle,

		HoldConnectorWidth,
		ConnectorWidth,

		ShowSpeeedChange,

		HiColor,
		KoriColor,
		GreenBitchColor,

		// osu cant save color yet
		_HiColorR,
		_KoriColorR,
		_GreenBitchColorR,
		_HiColorG,
		_KoriColorG,
		_GreenBitchColorG,
		_HiColorB,
		_KoriColorB,
		_GreenBitchColorB
	}

	public enum DashStyle {
		[LocalisableDescription( typeof( Localisation.Setting.RingStrings ), nameof( Localisation.Setting.RingStrings.Dashed ) )]
		Dashed,

		[LocalisableDescription( typeof( Localisation.Setting.RingStrings ), nameof( Localisation.Setting.RingStrings.Dotted ) )]
		Dotted,

		[LocalisableDescription( typeof( Localisation.Setting.RingStrings ), nameof( Localisation.Setting.RingStrings.Solid ) )]
		Solid
	}
}
