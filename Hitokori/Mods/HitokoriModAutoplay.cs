using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Hitokori.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Users;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModAutoplay : ModAutoplay {
		public override LocalisableString Description => "Let the cute bot do all the hard work";

		public override ModReplayData CreateReplayData ( IBeatmap beatmap, IReadOnlyList<Mod> mods ) {
			return new ModReplayData(
				new HitokoriReplayGenerator( beatmap ).Generate(),
				new ModCreatedUser { Username = "Autosu" }
			);
		}
	}
}
