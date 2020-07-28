using osu.Game.Rulesets.Mods;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModDaycore : ModDaycore {
		public override string Description => "Ice, ice baby";

		public override double ScoreMultiplier => 0.3;
	}
}
