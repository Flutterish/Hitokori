using Humanizer;
using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Replays;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Replays {
	public class HitokoriReplayInputHandler : FramedReplayInputHandler<HitokoriReplayFrame> { // TODO: BUG replays broke backwards
		public HitokoriReplayInputHandler ( Replay replay ) : base( replay ) { }

		public override void CollectPendingInputs ( List<IInput> inputs ) {
			inputs.Add( new ReplayState<HitokoriAction> {
				PressedActions = CurrentFrame?.Actions ?? new List<HitokoriAction>()
			} );
		}
	}
}
