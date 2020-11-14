using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Replays {
	public class HitokoriReplayFrame : ReplayFrame, IConvertibleReplayFrame {
		public List<HitokoriAction> Actions;

		public HitokoriReplayFrame () {

		}
		public HitokoriReplayFrame ( double time, List<HitokoriAction> actions ) : base( time ) {
			Actions = actions;
		}

		public void FromLegacy ( LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame lastFrame = null ) {
			Time = currentFrame.Time;

			Actions = new List<HitokoriAction>();
			if ( currentFrame.MouseLeft1 ) Actions.Add( HitokoriAction.Action1 );
			if ( currentFrame.MouseRight1 ) Actions.Add( HitokoriAction.Action2 );
		}

		public LegacyReplayFrame ToLegacy ( IBeatmap beatmap ) {
			ReplayButtonState state = ReplayButtonState.None;

			if ( Actions.Contains( HitokoriAction.Action1 ) ) state |= ReplayButtonState.Left1;
			if ( Actions.Contains( HitokoriAction.Action2 ) ) state |= ReplayButtonState.Right1;

			return new LegacyReplayFrame( Time, null, null, state );
		}
	}
}
