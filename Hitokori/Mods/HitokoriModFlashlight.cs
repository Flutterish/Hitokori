using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModFlashlight : ModFlashlight<HitokoriHitObject>, IUpdatableByPlayfield {
		public override double ScoreMultiplier => 1.12;
		public override string Description => "The fire dims";

		public override bool HasImplementation => true;

		HitokoriFlashlight flashlight;
		const float FLASHLIGHT_SIZE = 200;

		public override Flashlight CreateFlashlight ()
			=> flashlight = new HitokoriFlashlight();

		public void Update ( Playfield playfield ) {
			flashlight?.Update( playfield );
		}

		private class HitokoriFlashlight : Flashlight {
			public HitokoriFlashlight () {
				FlashlightSize = new Vector2( 0, FLASHLIGHT_SIZE );
			}

			protected override string FragmentShader => "CircularFlashlight";

			private float getSizeFor ( int combo ) {
				if ( combo > 200 )
					return FLASHLIGHT_SIZE * 0.8f;
				else if ( combo > 100 )
					return FLASHLIGHT_SIZE * 0.9f;
				else
					return FLASHLIGHT_SIZE;
			}

			protected override void OnComboChange ( ValueChangedEvent<int> e ) {
				this.TransformTo( nameof( FlashlightSize ), new Vector2( 0, getSizeFor( e.NewValue ) ), FLASHLIGHT_FADE_DURATION );
			}

			HitokoriPlayfield Playfield;
			public void Update ( Playfield playfield ) {
				Playfield = playfield as HitokoriPlayfield;

				FlashlightPosition = Playfield.Everything.ToParentSpace( Playfield.Hitokori.Position + Playfield.Hitokori.HiOffset );
			}
		}
	}
}
