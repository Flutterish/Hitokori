﻿using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Replays;
using System.Collections.Generic;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.Replays {
	public class HitokoriReplayInputHandler : FramedReplayInputHandler<HitokoriReplayFrame> {
		public HitokoriReplayInputHandler ( Replay replay ) : base( replay ) { }

		protected override bool IsImportant ( HitokoriReplayFrame frame ) => true;
		public override void CollectPendingInputs ( List<IInput> inputs ) {
			inputs.Add( new ReplayState<HitokoriAction> {
				PressedActions = CurrentFrame?.Actions ?? new List<HitokoriAction>()
			} );
		}
	}
}