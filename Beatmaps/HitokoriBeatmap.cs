using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmap : Beatmap<HitokoriHitObject> {
		public override IEnumerable<BeatmapStatistic> GetStatistics () {
			var holds = HitObjects.OfType<HoldTile>();
			var tiles = HitObjects.OfType<TapTile>();

			return new[]
			{
				new BeatmapStatistic
				{
					Name = @"Press Tiles",
					Content = tiles.Count().ToString(),
					Icon = FontAwesome.Solid.Square
				},
				new BeatmapStatistic
				{
					Name = @"Hold Tiles",
					Content = holds.Count().ToString(),
					Icon = FontAwesome.Solid.Circle
				}
			};
		}
	}
}
