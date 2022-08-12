// This file is auto-generated
// Do not edit it manually as it will be overwritten

using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Hitokori.Localisation.Mod {
	public static class FlashlightStrings {
		private const string PREFIX = "osu.Game.Rulesets.Hitokori.Localisation.Mod.Flashlight.Strings";
		private static string getKey( string key ) => $"{PREFIX}:{key}";

		/// <summary>
		/// Change size based on combo
		/// </summary>
		public static readonly LocalisableString ChangeSize = new TranslatableString(
			getKey( "change-size" ),
			"Change size based on combo"
		);

		/// <summary>
		/// Decrease the flashlight size as combo increases
		/// </summary>
		public static readonly LocalisableString ChangeSizeTooltip = new TranslatableString(
			getKey( "change-size-tooltip" ),
			"Decrease the flashlight size as combo increases"
		);

		/// <summary>
		/// Flashlight size
		/// </summary>
		public static readonly LocalisableString Size = new TranslatableString(
			getKey( "size" ),
			"Flashlight size"
		);

		/// <summary>
		/// Multiplier applied to the default flashlight size
		/// </summary>
		public static readonly LocalisableString SizeTooltip = new TranslatableString(
			getKey( "size-tooltip" ),
			"Multiplier applied to the default flashlight size"
		);
	}
}
