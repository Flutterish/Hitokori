using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects {
	public class TapTile : HitokoriTileObject {
		public TapTile () {
			PressPoint = new TilePoint();
		}

		public override DrawableHitokoriHitObject AsDrawable ()
			=> null;

		public TilePoint PressPoint;

		public override IEnumerable<TilePoint> AllTiles => PressPoint.Yield();

		public double PressTime {
			get => base.StartTime;
			set {
				base.StartTime = value;
				PressPoint.HitTime = value;
			}
		}
	}
}
