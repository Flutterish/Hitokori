using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osuTK;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Replays {
    public class HitokoriReplayRecorder : ReplayRecorder<HitokoriAction>
    {
		public HitokoriReplayRecorder ( Score target ) : base( target ) { }

		protected override ReplayFrame HandleFrame ( Vector2 mousePosition, List<HitokoriAction> actions, ReplayFrame previousFrame ) {
			HitokoriReplayFrame previous = previousFrame as HitokoriReplayFrame;
			if ( previous is null ) {
				return new HitokoriReplayFrame( Time.Current, actions.ToArray().ToList() );
			}
			else if ( ( actions.Count() != previous.Actions.Count ) || actions.Except( previous.Actions ).Any() ) {
				return new HitokoriReplayFrame( Time.Current, actions.ToArray().ToList() );
			}
			return null;
		}
	}
}
