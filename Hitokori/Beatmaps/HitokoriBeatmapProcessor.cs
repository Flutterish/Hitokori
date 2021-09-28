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

		public double? ForcedAnglePerBeat = null;
		public int? ForcedOrbitalCount = null;

		private void processChain ( IEnumerable<TilePoint> tiles ) {
			var anglePerBeat = ForcedAnglePerBeat ?? 180;
			var orbitalCount = ForcedOrbitalCount ?? 2;
			var interiorAngle = ( ( orbitalCount - 2 ) * 180d ) / orbitalCount;

			if ( ForcedOrbitalCount is not null ) {
				anglePerBeat = anglePerBeat - interiorAngle;
			}

			var distancePerBeat = anglePerBeat * 2 / 180 * Math.PI;
			TilePoint? prevTile = tiles.FirstOrDefault();

			int direction = prevTile is PassThroughTilePoint ? -1 : 1;

			foreach ( var tile in tiles.Skip( 1 ) ) {
				var connector = new TilePointRotationConnector {
					From = prevTile!,
					To = tile,
					BPM = (float)Beatmap.ControlPointInfo.TimingPointAt( prevTile!.StartTime ).BPM,
					DistancePerBeat = distancePerBeat,
					TargetOrbitalIndex = direction
				};

				if ( tile is PassThroughTilePoint ) {
					direction *= -1;
				}
				else if ( tile is SwapTilePoint && Math.Abs( connector.Angle ) <= ( 80d - interiorAngle / 2 ) / 180 * Math.PI ) {
					connector.TargetOrbitalIndex *= -1;
					direction *= -1;
				}

				prevTile = tile;
			}
		}
	}
}
