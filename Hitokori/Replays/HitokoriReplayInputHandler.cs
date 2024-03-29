﻿using JetBrains.Annotations;
using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Replays;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Replays {
	public class HitokoriReplayInputHandler : FramedReplayInputHandler<HitokoriReplayFrame> {
		public HitokoriReplayInputHandler ( Replay replay ) : base( replay ) { }

		protected override bool IsImportant ( [NotNull] HitokoriReplayFrame frame ) => true;
		protected override void CollectReplayInputs ( List<IInput> inputs ) {
			inputs.Add( new ReplayState<HitokoriAction> {
				PressedActions = CurrentFrame?.Actions ?? new List<HitokoriAction>()
			} );
		}
	}
}
