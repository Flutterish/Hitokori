using osu.Game.Rulesets.Mods;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModHardRock : ModHardRock {
		public override double ScoreMultiplier => 1.2;
		public override string Description => "Through the fire and the flames you go";

		public override bool HasImplementation => false;
	}
}
