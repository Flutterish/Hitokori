using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModFlashlight : ModFlashlight<HitokoriHitObject>, IUpdatableByPlayfield {
		public override double ScoreMultiplier => 1.12;
		public override LocalisableString Description => Localisation.ModStrings.FlashlightDescription;

		public override bool HasImplementation => true;

		HitokoriFlashlight flashlight;

		protected override Flashlight CreateFlashlight ()
			=> flashlight = new HitokoriFlashlight( this );

		public void Update ( Playfield playfield ) {
			flashlight?.Update( playfield );
		}

		private class HitokoriFlashlight : Flashlight {
			float FLASHLIGHT_SIZE => (float)Math.Max( ( Playfield?.Hitokori.Radius.Length ?? 200 ), 200 ) * 1.1f;
			public HitokoriFlashlight ( HitokoriModFlashlight mod ) : base( mod ) {
				FlashlightSize = new Vector2( 0, FLASHLIGHT_SIZE );
			}

			protected override string FragmentShader => "CircularFlashlight";

			private float getSizeFor ( int combo ) {
				if ( combo > 200 )
					return 0.8f;
				else if ( combo > 100 )
					return 0.9f;
				else
					return 1;
			}

			HitokoriPlayfield Playfield;
			public void Update ( Playfield playfield ) {
				Playfield = playfield as HitokoriPlayfield;

				FlashlightPosition = Playfield.Hitokori.Hi.ToSpaceOfOtherDrawable( Vector2.Zero, Playfield );
				FlashlightSize = new Vector2( FLASHLIGHT_SIZE * (float)comboModifier.Value );
			}

			Bindable<double> comboModifier = new();
			protected override void OnComboChange ( ValueChangedEvent<int> e ) {
				this.TransformBindableTo( comboModifier, getSizeFor( e.NewValue ), FLASHLIGHT_FADE_DURATION );
			}
		}

		public override BindableFloat SizeMultiplier { get; } = new( 1 ) { MinValue = 0.5f, MaxValue = 2, Precision = 0.1f };
		public override BindableBool ComboBasedSize { get; } = new( true );
		public override float DefaultFlashlightSize { get; } = 1;
	}
}
