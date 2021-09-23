using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Replays;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Replays {
	public class HitokoriReplayGenerator : AutoGenerator<HitokoriReplayFrame> {
		public HitokoriReplayGenerator ( IBeatmap beatmap ) : base( beatmap ) { }

		protected override void GenerateFrames () {
			Frames.Clear();

			double lastPressTime = ( Beatmap.HitObjects.FirstOrDefault()?.StartTime ?? 0 ) - 1000;
			var actions = Enum.GetValues<HitokoriAction>();
			int actionIndex = 0;


			void nextRelease ( double time ) {
				if ( time - lastPressTime >= KEY_UP_DELAY ) {
					Frames.Add( new HitokoriReplayFrame( lastPressTime + KEY_UP_DELAY, Array.Empty<HitokoriAction>() ) );
				}
			}

			void nextPress ( double time ) {
				Frames.Add( new HitokoriReplayFrame( lastPressTime = time, new HitokoriAction[] { actions[ actionIndex++ % actions.Length ] } ) );
			}

			Frames.Add( new HitokoriReplayFrame( lastPressTime, Array.Empty<HitokoriAction>() ) );
			foreach ( var ho in Beatmap.HitObjects ) {
				nextRelease( ho.StartTime );

				nextPress( ho.StartTime );
			}

			Frames.Add( new HitokoriReplayFrame( lastPressTime + KEY_UP_DELAY, Array.Empty<HitokoriAction>() ) );
		}
	}
}
