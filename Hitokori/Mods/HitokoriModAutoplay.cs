﻿using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Hitokori.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModAutoplay : ModAutoplay {
		public override string Description => "Let the cute bot do all the hard work";

		public override Score CreateReplayScore ( IBeatmap beatmap, IReadOnlyList<Mod> mods ) {
			return new Score {
				ScoreInfo = new ScoreInfo {
					User = new APIUser { Username = "Autosu" }
				},
				Replay = new HitokoriReplayGenerator( beatmap ).Generate()
			};
		}
	}
}
