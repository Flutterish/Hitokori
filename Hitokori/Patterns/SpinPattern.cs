using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Patterns.Selectors;
using osu.Game.Rulesets.Hitokori.Utils;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Patterns {
	public class SpinPattern : Pattern<HitokoriTileObject> {
		public override RangeSelector<HitokoriTileObject> GetSelector ()
			=> new WhereSelector<HitokoriTileObject>(
				( x, all ) => all.Any()
				? x is TapTile next && ( all.First() as TapTile ).PressPoint.Duration.IsAbout( next.PressPoint.Duration, 50 )
				: x is TapTile
			).AtLeast( 10 );

		public override IEnumerable<HitokoriTileObject> Apply ( IEnumerable<HitokoriTileObject> selected ) {
			var spin = new SpinTile {
				StartTime = selected.First().StartTime,
				EndTime = selected.Last().StartTime,
				TileAmout = selected.Count(),
				Samples = selected.SelectMany( x => x.Samples ).ToList()
			};

			foreach ( var i in spin.TilePoints ) {
				i.IsClockwise = selected.First().LastPoint.IsClockwise;
				i.AllowDynamicDistance = false;
			}

			return spin.Yield();
		}
	}
}
