using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Patterns.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Patterns {
	public class UntanglePattern : Pattern<HitokoriTileObject> {
		const int TANGLE_LENGHT = 8;
		const double REQUIED_IMPROVEMENT = 0.3;

		public override RangeSelector<HitokoriTileObject> GetSelector ()
			=> new WhereSelector<HitokoriTileObject>(
				tile => {
					var first = tile.FirstPoint;
					if ( first.AngleOffset >= Math.PI / 3 ) return false;
					var previous = first.GetPrevious( TANGLE_LENGHT );

					var normalDistance = previous.Sum( tile => ( tile.NormalizedTilePosition - first.NormalizedTilePosition ).Length );
					var flippedDistance = previous.Sum( tile => ( tile.NormalizedTilePosition - first.FlippedNormalizedTilePosition ).Length );

					return ( flippedDistance - normalDistance ) / normalDistance > REQUIED_IMPROVEMENT;
				}
			);

		public override IEnumerable<HitokoriTileObject> Apply ( IEnumerable<HitokoriTileObject> selected ) {
			foreach ( var i in selected ) {
				var tile = i.FirstPoint.Previous;
				tile.ChangedDirection = !tile.ChangedDirection;
			}

			return selected;
		}

		public override bool IsUnique => false;
	}
}
