using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Replays;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Replays {
	public class HitokoriReplayFrame : ReplayFrame {
		public List<HitokoriAction> Actions;
		public HitokoriReplayFrame ( double time, List<HitokoriAction> actions ) : base( time ) {
			Actions = actions;
		}
	}
}
