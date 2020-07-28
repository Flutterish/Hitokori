using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Patterns.Selectors;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Patterns {
	public class DoubleTilePattern : Pattern<HitokoriTileObject> {
		public override RangeSelector<HitokoriTileObject> GetSelector ()
			=> new WhereSelector<HitokoriTileObject>( x => x is TapTile tile && tile.Next is TapTile );

		public override IEnumerable<HitokoriTileObject> Apply ( IEnumerable<HitokoriTileObject> selected ) {
			List<TapTile> tiles = new List<TapTile>();

			foreach ( TapTile tile in selected ) {
				tiles.Add( tile );
				tiles.Add( new TapTile { 
					Samples = tile.Samples,
					PressTime = ( tile.PressTime + ( tile.Next as TapTile ).PressTime ) / 2
				} );
			}

			return tiles;
		}

		public override bool IsUnique => false;
	}
}
