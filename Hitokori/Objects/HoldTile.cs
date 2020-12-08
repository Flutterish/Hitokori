using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Objects.Types;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects {
	public class HoldTile : HitokoriTileObject, IHasDuration {
		public HoldTile () {
			StartPoint = new TilePoint();
			EndPoint = StartPoint.Then( new TilePoint() );
		}

		public override DrawableHitokoriHitObject AsDrawable ()
			=> null;

		public override void StylizeTiles () {
			EndPoint.Distance = StartPoint.Distance;
		}

		public TilePoint StartPoint;
		public TilePoint EndPoint;

		public override IEnumerable<TilePoint> AllTiles => new[] { StartPoint, EndPoint };

		public double EndTime {
			get => EndPoint.HitTime;
			set => EndPoint.HitTime = value;
		}
		new public double StartTime {
			get => base.StartTime;
			set {
				var duration = ( this as IHasDuration ).Duration;
				base.StartTime = value;
				StartPoint.HitTime = value;
				( this as IHasDuration ).Duration = duration;
			}
		}
		double IHasDuration.Duration {
			get => StartPoint.Duration;
			set => EndPoint.HitTime = StartPoint.HitTime + value;
		}
	}
}
