using osu.Game.Replays;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.UI;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Replays {
	public class HitokoriReplayRecorder : ReplayRecorder<HitokoriAction> {

		private readonly HitokoriPlayfield playfield;

		public HitokoriReplayRecorder ( Replay target, HitokoriPlayfield playfield ) : base( target ) {
			this.playfield = playfield;
		}

		protected override ReplayFrame HandleFrame ( Vector2 mousePosition, List<HitokoriAction> actions, ReplayFrame previousFrame ) {
			HitokoriReplayFrame previous = previousFrame as HitokoriReplayFrame;
			if ( previous is null ) {
				return new HitokoriReplayFrame( Time.Current, actions.ToArray().ToList() );
			} else if ( ( actions.Count() != previous.Actions.Count ) || actions.Except( previous.Actions ).Any() ) {
				return new HitokoriReplayFrame( Time.Current, actions.ToArray().ToList() );
			}
			return null;
		}
	}
}
