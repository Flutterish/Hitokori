using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using System;

namespace osu.Game.Rulesets.Hitokori.UI {
	public partial class ColorPickerControl {
		private class SVPicker : CompositeDrawable {
			public Bindable<float> HueBindable { get; init; }
			public Bindable<float> SaturationBindable { get; init; }
			public Bindable<float> ValueBindable { get; init; }
			public Bindable<bool> LockedBindable { get; init; }
			public Bindable<Color4> ColorBindable { get; init; }

			Sprite svBox;
			SVNotch svNotch;

			public SVPicker () {
				Size = new Vector2( PICKER_SIZE );
			}

			protected override void LoadComplete () {
				base.LoadComplete();
				HueBindable.BindValueChanged( v => {
					svBox.Texture.SetData( TextureGeneration.HSVBoxWithSetHue( (int)PICKER_SIZE, (int)PICKER_SIZE, v.NewValue ) );
				}, true );

				LockedBindable.BindValueChanged( v => {
					this.FadeColour( v.NewValue ? Colour4.Gray : Colour4.White, 70 );
				}, true );

				ColorBindable.BindValueChanged( v => {
					svNotch.box.Colour = v.NewValue;
				}, true );

				SaturationBindable.BindValueChanged( v => {
					svNotch.MoveTo( new Vector2( SaturationBindable.Value, 1 - ValueBindable.Value ), 70 );
				}, true );
				ValueBindable.BindValueChanged( v => {
					svNotch.MoveTo( new Vector2( SaturationBindable.Value, 1 - ValueBindable.Value ), 70 );
				}, true );
			}

			Sample notchSample;
			[BackgroundDependencyLoader]
			private void load ( ISampleStore samples, IRenderer renderer ) {
				notchSample = samples.Get( "UI/notch-tick" );

				InternalChildren = new Drawable[] {
					svBox = new Sprite {
						RelativeSizeAxes = Axes.Both,
						Texture = renderer.CreateTexture( (int)PICKER_SIZE, (int)PICKER_SIZE )
					},
					svNotch = new SVNotch {
						Origin = Anchor.Centre,
						RelativePositionAxes = Axes.Both
					}
				};
			}

			protected override bool OnDragStart ( DragStartEvent e ) {
				return !LockedBindable.Value;
			}

			protected override void OnDrag ( DragEvent e ) {
				if ( LockedBindable.Value ) return;
				var pos = Parent.ToSpaceOfOtherDrawable( e.MousePosition, this );
				updateAt( pos );
			}

			protected override bool OnClick ( ClickEvent e ) {
				if ( LockedBindable.Value ) return false;
				var pos = Parent.ToSpaceOfOtherDrawable( e.MousePosition, this );
				updateAt( pos );
				return true;
			}

			double sampleDelay = 120;
			double sampleTimer;
			void updateAt ( Vector2 pos ) {
				SaturationBindable.Value = Math.Clamp( pos.X / DrawWidth, 0, 1 );
				ValueBindable.Value = Math.Clamp( 1 - ( pos.Y / DrawHeight ), 0, 1 );

				if ( sampleTimer > sampleDelay ) {
					notchSample.Play();
					sampleTimer = 0;
				}
			}

			protected override void Update () {
				base.Update();
				sampleTimer += Time.Elapsed;
			}

            private class SVNotch : CompositeDrawable
            {
				public Box box;
				public SVNotch () {
					AddInternal( box = new Box {
						RelativeSizeAxes = Axes.Both,
						AlwaysPresent = true
					} );

					Size = new Vector2( 14 );
					Masking = true;
					CornerRadius = Width / 2;
					BorderColour = Color4.Black;
					BorderThickness = 3;
				}
			}
		}
	}
}
