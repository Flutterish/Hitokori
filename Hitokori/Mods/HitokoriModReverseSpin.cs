﻿using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModReverseSpin : Mod {
		public override string Name => "Reverse Spin";
		public override string Acronym => "RS";
		public override LocalisableString Description => Localisation.ModStrings.ReverseSpinDescription;

		public override double ScoreMultiplier => 1;

		public override IconUsage? Icon => FontAwesome.Solid.Recycle;

		public override ModType Type => ModType.Fun;

		public override bool HasImplementation => true;
	}
}
