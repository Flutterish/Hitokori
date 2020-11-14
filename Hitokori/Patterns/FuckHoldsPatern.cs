using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Patterns.Selectors;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Patterns {
	public class FuckHoldsPatern : Pattern<HitokoriTileObject> {
		public override RangeSelector<HitokoriTileObject> GetSelector ()
			=> new WhereSelector<HitokoriTileObject>( x => x is HoldTile );

		public override IEnumerable<HitokoriTileObject> Apply ( IEnumerable<HitokoriTileObject> selected ) {
			List<HitokoriTileObject> all = new List<HitokoriTileObject>();

			foreach ( HoldTile hold in selected ) {
				all.Add( new TapTile { PressTime = hold.StartTime, Samples = hold.Samples[ 0 ].Yield().ToList() } );
				all.Add( new TapTile { PressTime = hold.EndTime, Samples = hold.Samples[ 1 ].Yield().ToList() } );
			}

			return all;
		}
	}
}
