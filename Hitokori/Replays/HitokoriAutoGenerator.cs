using osu.Game.Replays;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Replays;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Replays {
	public class HitokoriAutoGenerator : AutoGenerator {
		new public HitokoriBeatmap Beatmap => base.Beatmap as HitokoriBeatmap;
		public HitokoriAutoGenerator ( HitokoriBeatmap beatmap ) : base( beatmap ) { }

		public override Replay Generate () {
			var Replay = new Replay();
			List<HitokoriAction> pressed = new List<HitokoriAction>();

			int index = 0;
			HitokoriAction NextAction () {
				return ( index++ % 2 == 0 ) ? HitokoriAction.Action1 : HitokoriAction.Action2;
			}

			double lastTime = -10000;
			void TryToReleaseBefore ( double time ) {
				if ( pressed.Count > 0 ) {
					pressed.Clear();
					if ( time > lastTime + KEY_UP_DELAY ) {
						ReleaseAt( lastTime + KEY_UP_DELAY );
					}
				}
			}

			void PressAt ( double time ) {
				TryToReleaseBefore( time );

				pressed.Add( NextAction() );
				Replay.Frames.Add( new HitokoriReplayFrame( time, pressed.ToList() ) );
				lastTime = time;
			}

			void HoldAt ( double time ) {
				TryToReleaseBefore( time );

				pressed.Add( NextAction() );
				Replay.Frames.Add( new HitokoriReplayFrame( time, pressed.ToList() ) );
				lastTime = time;
			}

			void ReleaseAt ( double time ) {
				pressed.Clear();

				Replay.Frames.Add( new HitokoriReplayFrame( time, pressed.ToList() ) );
				lastTime = time;
			}

			foreach ( var i in Beatmap.HitObjects.OfType<HitokoriTileObject>().OrderBy( x => x.StartTime ) ) {
				switch ( i ) {
					case TapTile tap:
						PressAt( tap.StartTime );
						break;

					case HoldTile hold:
						HoldAt( hold.StartTime );
						ReleaseAt( hold.EndTime );
						break;

					case SpinTile spin:
						foreach ( var tap in spin.TilePoints ) {
							PressAt( tap.HitTime );
						}
						break;
				}
			}

			TryToReleaseBefore( lastTime + 1000 );

			return Replay;
		}
	}
}
