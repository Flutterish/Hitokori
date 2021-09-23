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

			int nextButtonIndex = 0;
			var buttons = new AutoButton<HitokoriAction>[] { 
				new( HitokoriAction.Action1 ), 
				new( HitokoriAction.Action2 )
			};

			void nextFrame ( double time ) {
				Frames.Add( new HitokoriReplayFrame( time, buttons.Where( x => x.IsDown ).Select( x => x.Action ) ) );
			}

			nextFrame( ( Beatmap.HitObjects.FirstOrDefault()?.StartTime ?? 0 ) - 1000 );
			foreach ( var ho in Beatmap.HitObjects ) {
				var button = buttons[ nextButtonIndex++ % buttons.Length ];
				var releases = buttons
					.Where( x => x.IsDown )
					.GroupBy( x => x == button ? Math.Min( ho.StartTime, x.PressTime + KEY_UP_DELAY ) : ( x.PressTime + KEY_UP_DELAY ) )
					.OrderBy( x => x.Key );

				foreach ( var i in releases ) {
					if ( ho.StartTime >= i.Key ) {
						foreach ( var k in i ) {
							k.IsDown = false;
						}
						nextFrame( i.Key );
					}
				}

				button.IsDown = true;
				button.PressTime = ho.StartTime;

				nextFrame( ho.StartTime );
			}

			var finalReleases = buttons
				.Where( x => x.IsDown )
				.GroupBy( x => x.PressTime + KEY_UP_DELAY )
				.OrderBy( x => x.Key );

			foreach ( var i in finalReleases ) {
				foreach ( var k in i ) {
					k.IsDown = false;
				}
				nextFrame( i.Key );
			}
		}
	}
}
