using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Difficulty;
using osu.Game.Rulesets.Hitokori.Mods;
using osu.Game.Rulesets.Hitokori.Replays;
using osu.Game.Rulesets.Hitokori.Scoring;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Ranking.Statistics;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori {
	public class HitokoriRuleset : Ruleset {
		public const string SHORT_NAME = "hitokori";
		public override string Description => SHORT_NAME;
		public override string ShortName => SHORT_NAME;
		public override string PlayingVerb => "Playing with fire";

		public override Drawable CreateIcon ()
			=> new HitokoriIcon();

		public override IBeatmapConverter CreateBeatmapConverter ( IBeatmap beatmap )
			=> new HitokoriBeatmapConverter( beatmap, this );

		public override IRulesetConfigManager CreateConfig ( SettingsStore settings )
			=> new HitokoriSettingsManager( settings, RulesetInfo );
		public override RulesetSettingsSubsection CreateSettings ()
			=> new HitokoriSettingsSubsection( this );

		public override DifficultyCalculator CreateDifficultyCalculator ( IWorkingBeatmap beatmap )
			=> new HitokoriDifficultyCalculator( RulesetInfo, beatmap );
		public override HealthProcessor CreateHealthProcessor ( double drainStartTime )
			=> new HitokoriHealthProcessor();
		public override ScoreProcessor CreateScoreProcessor () {
			return base.CreateScoreProcessor();
		}
		public override StatisticItem[] CreateStatisticsForScore ( ScoreInfo score, IBeatmap playableBeatmap ) {
			return base.CreateStatisticsForScore( score, playableBeatmap );
		}

		public override DrawableRuleset CreateDrawableRulesetWith ( IBeatmap beatmap, IReadOnlyList<Mod> mods = null )
			=> new HitokoriDrawableRuleset( this, beatmap, mods );

		public override IConvertibleReplayFrame CreateConvertibleReplayFrame ()
			=> new HitokoriReplayFrame();

		public HitokoriRuleset () {
			void RegisterMods ( IEnumerable<Mod> mods ) {
				foreach ( var mod in mods ) {
					if ( mod is MultiMod multi ) {
						RegisterMods( multi.Mods );
					}
					else {
						ModCompatibility.RegisterMod( GetType(), mod.GetType() );
					}
				}
			}

			foreach ( ModType type in Enum.GetValues( typeof( ModType ) ) ) {
				RegisterMods( GetModsFor( type ) );
			}
		}
		public override IEnumerable<Mod> GetModsFor ( ModType type ) {
			switch ( type ) {
				case ModType.DifficultyReduction:
					return new Mod[] {
						new HitokoriModEasy(),
						new HitokoriModNoFail(),
						new MultiMod( new HitokoriModHalfTime(), new HitokoriModDaycore() )
					};

				case ModType.DifficultyIncrease:
					return new Mod[] {
						new HitokoriModHardRock(),
						new MultiMod( new HitokoriModSuddenDeath(), new HitokoriModPerfect() ),
						new MultiMod( new HitokoriModDoubleTime(), new HitokoriModNightcore() ),
						new HitokoriModHidden(),
						new HitokoriModFlashlight()
					};

				case ModType.Conversion:
					return new Mod[] {
						new HitokoriModHoldTiles(),
						new HitokoriModSpinTiles(),
						new HitokoriModUntangle()
					};

				case ModType.Automation:
					return new Mod[] {
						new HitokoriModAuto()
					};

				case ModType.Fun:
					return new Mod[] {
						new HitokoriModReverseSpin(),
						new HitokoriModTriplets()
					};

				case ModType.System:
					return Array.Empty<Mod>();

				default:
					return Array.Empty<Mod>();
			}
		}

		public override IEnumerable<KeyBinding> GetDefaultKeyBindings ( int variant = 0 ) {
			return new[] {
				new KeyBinding( InputKey.Z, HitokoriAction.Action1 ),
				new KeyBinding( InputKey.MouseLeft, HitokoriAction.Action1 ),

				new KeyBinding( InputKey.X, HitokoriAction.Action2 ),
				new KeyBinding( InputKey.MouseRight, HitokoriAction.Action2 )
			};
		}

		public override LocalisableString GetDisplayNameForHitResult ( HitResult result ) {
			return result switch
			{
				HitResult.Miss => Localisation.JudgementStrings.Miss,
				HitResult.Great => Localisation.JudgementStrings.Late,
				HitResult.Ok => Localisation.JudgementStrings.Early,
				_ => Localisation.JudgementStrings.Perfect
			};
		}

		protected override IEnumerable<HitResult> GetValidHitResults () {
			return new HitResult[] {
				HitResult.Miss,
				HitResult.Ok,
				HitResult.Great,
				HitResult.Perfect
			};
		}
	}
}
