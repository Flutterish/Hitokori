using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Replays;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Users;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModAuto : ModAutoplay {
		public override string Name => "Auto";
		public override string Acronym => "AT";
		public override string Description => "Let the cute bot do all the hard work";

		public override bool UserPlayable => false;

		public override double ScoreMultiplier => 1;

		public override IconUsage? Icon => OsuIcon.ModAuto;

		public override bool HasImplementation => true;

		public static readonly string[] BotNames = new[] {
			"Icy boi",
			"Cool dude",
			"Hot stuff",
			"Rob-bop",
			"Greg",
			"Rawr x3 *nuzzles* how are you *pounces on you* you're so warm o3o",
			"Megamind",
			"Yandere Dev",
			"Press Alt-F4 to save score",
			"Free Wi-fi anywhere you go",
			"Markiplier",
			"Pro gamer",
			"Vsauce Michael",
			"Ninja Fortnite",
			"Tyler1",
			"Bees?",
			"CallMeCarson",
			"KamilPL",
			"Harambe",
			"reddit moment",
			"cringe",
			"Keanu Chungus",
			"Brobama",
			"What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.",
			"Your mom",
			"An 11 y/o asian",
			"Jeffry Epstein",
			"Chaokobon",
			"Myahster",
			"Lizzy",
			"Corona man",
			"e621.net",
			"a gay potato",
			"github clout",
			"420 haha funny number",
			"Grand dad? Fleenstones?",
			"Simpleflips",
			"Godzilla himself",
			"Puro",
			"Pinkie Pie",
			"Shrek",
			"Sans Undertale",
			"Mozart",
			"Creeper? Aww man",
			"Peter Parkinson",
			"I want you to know, no matter what, I will never give you up, let you down or say goodbye.",
			"Help, I'm trapped inside a computer!",
			"Soundclown",
			"Yasuo main",
			"Duo",
			"Discord mods combined",
			"Jump King",
			"Merg"
		};

		public override Score CreateReplayScore ( IBeatmap beatmap, IReadOnlyList<Mod> mods)
		{
			var score = new Score
			{
				ScoreInfo = new ScoreInfo {User = new User {Username = BotNames.Random()}},
				Replay = new HitokoriAutoGenerator(beatmap as HitokoriBeatmap).Generate()
			};
			return score;
		}
	}
}
