using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmapProcessor : BeatmapProcessor {
		public HitokoriBeatmapProcessor ( IBeatmap beatmap ) : base( beatmap ) {
		}

		public override void PreProcess () {
			base.PreProcess();

			processChain( Beatmap.HitObjects.OfType<TilePoint>() );
		}

		private void processChain ( IEnumerable<TilePoint> tiles ) {
			TilePoint prevTile = tiles.FirstOrDefault();

			int direction = 1;

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

		private void processDuetChain ( IEnumerable<TilePoint> tiles ) {
			TilePoint prevTile = tiles.FirstOrDefault();

			int direction = 1;
			int count = 0;
			bool swap = false;

			foreach ( var tile in tiles.Skip( 1 ) ) {
				if ( count > 20 && tile is SwapTilePoint tp ) {
					swap = !swap;
					count = 0;
					tp.Position = prevTile.Position + new Vector2d( -2 );
					tp.OrbitalState = new Orbitals.OrbitalState( swap
						? new Vector2d[] {
							(Math.Tau * 1 / 3).AngleToVector(0.5),
							(Math.Tau * 2 / 3).AngleToVector(0.5),
							(Math.Tau * 3 / 3).AngleToVector(0.5)
						} : new Vector2d[] {
							(0.0).AngleToVector(0.5),
							(Math.PI).AngleToVector(0.5)
						}
					).PivotNth( 0, tp.Position );
					prevTile = tile;
					continue;
				}

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
				count++;
			}
		}
	}
}
