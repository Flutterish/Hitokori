﻿using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmap : Beatmap<HitokoriHitObject> {
		public bool IsLinked = false;
		public readonly BindableDictionary<int, Chain> Chains = new();

		public int CreateChain ( TilePoint root ) {
			int id = 0;
			while ( Chains.Keys.Any( x => x == id ) )
				id++;

			Chains.Add( id, new Chain( root, (id+1).ToSpreadsheetNotation() ) );

			return id;
		}

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
