using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmap : Beatmap<HitokoriHitObject> {
		public bool IsLinked = false;
		public readonly Dictionary<int, TilePoint> Chains = new();

		public override IEnumerable<BeatmapStatistic> GetStatistics () {
			yield return new BeatmapStatistic {
				Name = "Tiles",
				CreateIcon = () => new BeatmapStatisticIcon( BeatmapStatisticsIconType.Circles ),
				Content = $"{HitObjects.Count}"
			};

			yield return new BeatmapStatistic {
				Name = "Chains",
				CreateIcon = () => new BeatmapStatisticIcon( BeatmapStatisticsIconType.OverallDifficulty ),
				Content = $"{Chains.Count}"
			};
		}
	}
}
