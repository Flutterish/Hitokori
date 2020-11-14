using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects {
	public class FirstTilePoint : TilePoint {
		public FirstTilePoint () {
			WasHit = true;

			isPositionCached = true;
			cachedPosition = new Vector2();

			isOutAngleCached = true;
			cachedOutAngle = 0;
		}
	}
}
