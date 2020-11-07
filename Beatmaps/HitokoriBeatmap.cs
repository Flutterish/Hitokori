using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmap : Beatmap<HitokoriHitObject> {
		public override IEnumerable<BeatmapStatistic> GetStatistics () {
			yield return new BeatmapStatistic {
				Name = "Press Tiles",
				Content = HitObjects.OfType<TapTile>().Count().ToString(),
				CreateIcon = () => new SpriteIcon { Icon = FontAwesome.Solid.Square }
			};
			yield return new BeatmapStatistic {
				Name = "Hold Tiles",
				Content = HitObjects.OfType<HoldTile>().Count().ToString(),
				CreateIcon = () => new SpriteIcon { Icon = FontAwesome.Solid.HandHolding }
			};
			yield return new BeatmapStatistic {
				Name = "Spin Tiles",
				Content = HitObjects.OfType<SpinTile>().Count().ToString(),
				CreateIcon = () => new SpriteIcon { Icon = FontAwesome.Solid.RedoAlt }
			};
		}
	}
}
