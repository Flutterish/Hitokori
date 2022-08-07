using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmap : Beatmap<HitokoriHitObject> {
		public override IEnumerable<BeatmapStatistic> GetStatistics () {
			var taps = HitObjects.OfType<TapTile>();
			if ( taps.Any() ) yield return new BeatmapStatistic {
				Name = Localisation.Stats.Strings.PressTiles,
				Content = taps.Count().ToString(),
				CreateIcon = () => new SpriteIcon { Icon = FontAwesome.Solid.Circle }
			};
			var holds = HitObjects.OfType<HoldTile>();
			if ( holds.Any() ) yield return new BeatmapStatistic {
				Name = Localisation.Stats.Strings.HoldTiles,
				Content = holds.Count().ToString(),
				CreateIcon = () => new SpriteIcon { Icon = FontAwesome.Solid.HandHolding }
			};
			var spins = HitObjects.OfType<SpinTile>();
			if ( spins.Any() ) yield return new BeatmapStatistic {
				Name = Localisation.Stats.Strings.SpinTiles,
				Content = spins.Count().ToString(),
				CreateIcon = () => new SpriteIcon { Icon = FontAwesome.Solid.RedoAlt }
			};
		}
	}
}
