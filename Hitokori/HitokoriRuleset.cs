using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Edit.Compose;
using osu.Game.Rulesets.Hitokori.Edit.Setup;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Hitokori.Mods;
using osu.Game.Rulesets.Hitokori.Replays;
using osu.Game.Rulesets.Hitokori.Scoring;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit.Setup;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori {
	public class HitokoriRuleset : Ruleset {
		public const string SHORT_NAME = "hitokori";
		public override string Description => SHORT_NAME;
		public override string ShortName => SHORT_NAME;
		public override string PlayingVerb => "Playing with fire";

		public HitokoriRuleset () {
			
		}

		public override IEnumerable<Mod> GetModsFor ( ModType type ) {
			return type switch {
				ModType.DifficultyReduction => new Mod[] {
					new HitokoriModEasy(),
					new HitokoriModNoFail(),
					new MultiMod( 
						new HitokoriModHalfTime(), 
						new HitokoriModDaycore() 
					) 
				},
				ModType.DifficultyIncrease => new Mod[] {
					new MultiMod( 
						new HitokoriModSuddenDeath(), 
						new HitokoriModPerfect() 
					),
					new MultiMod( 
						new HitokoriModDoubleTime(),
						new HitokoriModNightcore()
					) 
				},
				ModType.Conversion => new Mod[] { 
					new HitokoriModAngles(),
					new HitokoriModOrbitals()
				},
				ModType.Automation => new Mod[] { 
					new HitokoriModAutoplay() 
				},
				ModType.Fun => new Mod[] {
					new HitokoriModSolo()
				},
				ModType.System => Array.Empty<Mod>(),

				_ => Array.Empty<Mod>()
			};
		}

		public override DrawableRuleset CreateDrawableRulesetWith ( IBeatmap beatmap, IReadOnlyList<Mod>? mods = null )
			=> new DrawableHitokoriRuleset( this, beatmap, mods );

		public override IBeatmapConverter CreateBeatmapConverter ( IBeatmap beatmap )
			=> new HitokoriBeatmapConverter( beatmap, this );
		public override IBeatmapProcessor CreateBeatmapProcessor ( IBeatmap beatmap )
			=> new HitokoriBeatmapProcessor( beatmap );

		public override HitObjectComposer CreateHitObjectComposer ()
			=> new HitokoriHitObjectComposer( this );
		public override RulesetSetupSection CreateEditorSetupSection ()
			=> new HitokoriSetupSection( RulesetInfo );

		public override DifficultyCalculator CreateDifficultyCalculator ( WorkingBeatmap beatmap )
			=> new HitokoriDifficultyCalculator( this, beatmap );

		public override IRulesetConfigManager CreateConfig ( SettingsStore settings )
			=> new HitokoriConfigManager( settings, RulesetInfo );
		public override RulesetSettingsSubsection CreateSettings ()
			=> new HitokoriSettingsSubsection( this );

		public override IConvertibleReplayFrame CreateConvertibleReplayFrame ()
			=> new HitokoriReplayFrame();

		public override IEnumerable<KeyBinding> GetDefaultKeyBindings ( int variant = 0 ) {
			return new KeyBinding[] {
				new( InputKey.Z, HitokoriAction.Action1 ),
				new( InputKey.X, HitokoriAction.Action2 ),
				new( InputKey.MouseLeft, HitokoriAction.Action1 ),
				new( InputKey.MouseRight, HitokoriAction.Action2 )
			};
		}

		public override string GetDisplayNameForHitResult ( HitResult result ) {
			return result switch {
				HitResult.Perfect => "Perfect",
				HitResult.Ok => "Ok",
				HitResult.Meh => "Meh",
				HitResult.Miss => "Miss",
				_ => $"Invalid {nameof(HitResult)}"
			};
		}

		protected override IEnumerable<HitResult> GetValidHitResults () {
			return new HitResult[] {
				HitResult.Perfect,
				HitResult.Ok,
				HitResult.Meh,
				HitResult.Miss
			};
		}

		public override Drawable CreateIcon ()
			=> new HitokoriIcon( this );

		private class HitokoriIcon : CompositeDrawable {
			private static LargeTextureStore? textureStore;
			private HitokoriRuleset ruleset;

			public HitokoriIcon ( HitokoriRuleset ruleset ) {
				Anchor = Origin = Anchor.Centre;
				this.ruleset = ruleset;
				FillAspectRatio = 1;
				FillMode = FillMode.Fit;
				Size = new Vector2( 100, 100 );
			}

			[BackgroundDependencyLoader]
			private void load ( GameHost host ) {
				textureStore ??= new LargeTextureStore( host.CreateTextureLoaderStore( ruleset.CreateResourceStore() ) );

				AddInternal( new Sprite {
					Anchor = Anchor.Centre,
					Origin = Anchor.Centre,
					RelativeSizeAxes = Axes.Both,
					Texture = textureStore.Get( "Textures/HitokoriIcon" ),
					EdgeSmoothness = new Vector2( 2 )
				} );
			}
		}
	}
}
