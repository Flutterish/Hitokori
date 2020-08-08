using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Patterns.Selectors;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Patterns {
	public class StairsPattern : Pattern<HitokoriTileObject> {
		public override RangeSelector<HitokoriTileObject> GetSelector ()
			=> new WhereSelector<HitokoriTileObject>(
				x => x is TapTile tile
				&& Math.Abs( tile.PressPoint.AngleOffset ) < Shape.Triangle.Angle()
			);

		public override IEnumerable<HitokoriTileObject> Apply ( IEnumerable<HitokoriTileObject> selected ) {
			foreach ( TapTile tile in selected ) {
				tile.PressPoint.IsClockwise = !tile.PressPoint.Previous.IsClockwise;
			}

			return selected;
		}
		public override bool IsUnique => false;
	}
}
