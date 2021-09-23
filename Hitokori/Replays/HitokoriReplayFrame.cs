using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.Replays {
	public class HitokoriReplayFrame : ReplayFrame, IConvertibleReplayFrame {
		public List<HitokoriAction> Actions = new();

		public HitokoriReplayFrame () { }
		public HitokoriReplayFrame ( double time, IEnumerable<HitokoriAction> actions ) : base( time ) {
			Actions = actions.ToList();
		}

		public void FromLegacy ( LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame? lastFrame = null ) {
			Time = currentFrame.Time;

			Actions.Clear();
			if ( currentFrame.MouseLeft )
				Actions.Add( HitokoriAction.Action1 );
			if ( currentFrame.MouseRight )
				Actions.Add( HitokoriAction.Action2 );
		}

		public LegacyReplayFrame ToLegacy ( IBeatmap beatmap ) {
			var buttonState = ReplayButtonState.None;

			if ( Actions.Contains( HitokoriAction.Action1 ) )
				buttonState |= ReplayButtonState.Left1;
			if ( Actions.Contains( HitokoriAction.Action2 ) )
				buttonState |= ReplayButtonState.Right1;

			return new LegacyReplayFrame( Time, null, null, buttonState );
		}
	}
}
