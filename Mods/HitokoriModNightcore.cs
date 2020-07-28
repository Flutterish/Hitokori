using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Mods;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModNightcore : ModNightcore<HitokoriHitObject> {
		public override double ScoreMultiplier => 1.12;
	}
}
