using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Patterns.Selectors;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Patterns {
	public class ReverseHoldPattern : Pattern<HitokoriTileObject> {
		public override RangeSelector<HitokoriTileObject> GetSelector ()
			=> new WhereSelector<HitokoriTileObject>( x => x is HoldTile hold );

		public override IEnumerable<HitokoriTileObject> Apply ( IEnumerable<HitokoriTileObject> selected ) {
			foreach ( HoldTile hold in selected ) {
				hold.StartPoint.IsClockwise = hold.StartPoint.Previous.IsClockwise;
				hold.EndPoint.IsClockwise = !hold.StartPoint.IsClockwise;
			}

			return selected;
		}
		public override bool IsUnique => false;
	}
}
