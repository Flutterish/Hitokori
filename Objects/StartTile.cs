using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects {
	public class StartTile : HitokoriTileObject { // TODO first tile should be the same velocity as the actual first tile
		// TODO: BUG sometimes ( long intros? ) if first tile is missed orbitals gain insane speed
		public override DrawableHitokoriHitObject AsDrawable () {
			throw new NotImplementedException( "You shouldn't draw a `StartTile` ( yet )" ); // TODO probably allow this but ignore judgement
		}

		public TilePoint HoldPoint;

		public override IEnumerable<TilePoint> AllTiles => HoldPoint.Yield();
	}
}
