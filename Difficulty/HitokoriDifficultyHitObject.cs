using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Difficulty {
	public class HitokoriDifficultyHitObject : DifficultyHitObject {
		public HitokoriDifficultyHitObject ( HitObject hitObject, HitObject lastObject, double clockRate ) : base( hitObject, lastObject, clockRate ) {

		}
	}
}
