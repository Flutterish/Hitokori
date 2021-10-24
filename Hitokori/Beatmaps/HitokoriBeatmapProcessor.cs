using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
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
		public GameVariant Variant = GameVariant.Default;

		private void processChain ( IEnumerable<TilePoint> tiles ) {
			switch ( Variant ) {
				case GameVariant.Default:
					processRegularChain( tiles );
					break;

				case GameVariant.TapTapDash:
					processTapTapDashChain( tiles );
					break;
			}
		}

		private void processTapTapDashChain ( IEnumerable<TilePoint> tiles ) {
			var random = new Random( Beatmap.Metadata.ID );

			double angle = 0;
			TilePoint? prevTile = tiles.FirstOrDefault();
			var angleDelta = (180 - (ForcedAnglePerBeat ?? 90)) / 180 * Math.PI;

			foreach ( var tile in tiles.Skip( 1 ) ) {
				var connector = ( prevTile is PassThroughTilePoint || tile is PassThroughTilePoint )
					? new JumpingTilePointLinearConnector {
						From = prevTile!,
						To = tile,
						BPM = (float)Beatmap.ControlPointInfo.TimingPointAt( prevTile!.StartTime ).BPM,
						TargetOrbitalIndex = 0,

						Angle = angle
					}
					: new TilePointLinearConnector {
						From = prevTile!,
						To = tile,
						BPM = (float)Beatmap.ControlPointInfo.TimingPointAt( prevTile!.StartTime ).BPM,
						TargetOrbitalIndex = 0,

						Angle = angle
					};

				if ( random.NextDouble() < 0.5 ) {
					angle += angleDelta;
				}
				else {
					angle -= angleDelta;
				}

				prevTile = tile;
			}
		}

		private void processRegularChain ( IEnumerable<TilePoint> tiles ) {
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
