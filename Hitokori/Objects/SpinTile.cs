using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Objects.Types;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects {
	public class SpinTile : HitokoriTileObject, IHasDuration {
		public List<TilePoint> TilePoints = new List<TilePoint>();

		public SpinTile () {
			RegenTiles();
		}

		new public double StartTime {
			get => base.StartTime;
			set {
				base.StartTime = value;
				RegenTiles();
			}
		}
		private double releaseTime;
		public double EndTime {
			get => releaseTime;
			set {
				releaseTime = value;
				RegenTiles();
			}
		}

		public double Duration {
			get => EndTime - StartTime;
			set => EndTime = StartTime + value;
		}

		private int tileAmout = 2;
		public int TileAmout {
			get => tileAmout;
			set {
				tileAmout = value;
				RegenTiles();
			}
		}

		public override IEnumerable<TilePoint> AllTiles => TilePoints;

		private void RegenTiles () {
			TilePoints.Clear();

			TilePoint last = null;
			for ( int i = 0; i < tileAmout; i++ ) {
				var angle = Math.PI * 2 / ( tileAmout - 1 ) * i - Math.PI / 2;

				var tile = new TilePoint {
					HitTime = StartTime + Duration / ( tileAmout - 1 ) * i,
					Distance = 1 - 0.75 * ( Math.Sin( angle ) + 1 ) / 2
				};

				TilePoints.Add( tile );
				if ( last != null ) last.Then( tile );

				last = tile;
			}
		}

		public override DrawableHitokoriHitObject AsDrawable ()
			=> null;
	}
}
