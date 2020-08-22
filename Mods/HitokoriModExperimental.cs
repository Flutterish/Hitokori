using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModExperimental : Mod, IApplicableToBeatmapConverter {
		public override string Name => "Experimental";
		public override string Acronym => "Ex";
		public override string Description => "Only for the bold";

		public override double ScoreMultiplier => 1;

		public override IconUsage? Icon => FontAwesome.Solid.Flask;

		public override bool HasImplementation => true;
		public override bool Ranked => false;

		public override ModType Type => ModType.Conversion;

		[SettingSource( "Replace hold tiles with 2 tap tiles" )]
		public BindableBool RemoveHoldTiles { get; } = new BindableBool {
			Value = false
		};

		[SettingSource( "Replace longer streams with spin tiles" )]
		public BindableBool GenerateSpinTiles { get; } = new BindableBool {
			Value = false
		};

		[SettingSource( "Remove unhitable objects (maps with overlapping notes)" )]
		public BindableBool RemoveUnhitable { get; } = new BindableBool {
			Value = false
		};

		public void ApplyToBeatmapConverter ( IBeatmapConverter beatmapConverter ) {
			( beatmapConverter as HitokoriBeatmapConverter ).NoHolds = RemoveHoldTiles.Value;
			( beatmapConverter as HitokoriBeatmapConverter ).GenerateSpins = GenerateSpinTiles.Value;
			( beatmapConverter as HitokoriBeatmapConverter ).NoUnhitable = RemoveUnhitable.Value;
		}

		public override string SettingDescription { // BUG "no mods" is always displayed on the leaderboard. on lazer's side.
			get {
				string removeHolds = RemoveHoldTiles.Value ? "Holds -> 2 Taps" : string.Empty;
				string makeSpins = GenerateSpinTiles.Value ? "Streams -> Spins" : string.Empty;
				string removeUnhitable = RemoveUnhitable.Value ? "No Unhitable" : string.Empty;

				string description = string.Join( ", ", new[] {
					removeHolds,
					makeSpins,
					removeUnhitable
				}.Where( s => !string.IsNullOrEmpty( s ) ) );

				return string.IsNullOrEmpty( description ) ? string.Empty : description;
			}
		}
	}
}
