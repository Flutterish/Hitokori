using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmapProcessor : BeatmapProcessor {
		public HitokoriBeatmapProcessor ( IBeatmap beatmap ) : base( beatmap ) {
		}

		public override void PreProcess () {
			base.PreProcess();

			foreach ( var chain in Beatmap.HitObjects.OfType<TilePoint>().GroupBy( x => x.ChainID ) ) {
				processChain( chain );
			}
		}

		private void processChain ( IEnumerable<TilePoint> tiles ) {
			TilePoint prevTile = tiles.FirstOrDefault();

			int direction = prevTile is PassThroughTilePoint ? -1 : 1;

			foreach ( var tile in tiles.Skip( 1 ) ) {
				var connector = new TilePointRotationConnector {
					From = prevTile,
					To = tile,
					BPM = (float)Beatmap.ControlPointInfo.TimingPointAt( prevTile.StartTime ).BPM,
					DistancePerBeat = 240f / 180 * MathF.PI,
					TargetOrbitalIndex = direction
				};

				if ( tile is PassThroughTilePoint )
					direction *= -1;

				prevTile = tile;
			}
		}
	}
}
