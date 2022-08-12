using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.UI {
	public partial class ColorPickerControl {
		private class HuePicker : CompositeDrawable {
			const int DISCRETE_COUNT = 12;

			static Texture continuousTexture;
			static Texture discreteTexture;

			public Bindable<float> HueBindable { get; init; }
			public Bindable<bool> LockedBindable { get; init; }

			Sample notchSample;

			[BackgroundDependencyLoader]
			private void load ( IRenderer renderer, ISampleStore samples ) {
				notchSample = samples.Get( "UI/notch-tick" );

				int scale = 3;
				float width = 12;
				float radius = MathF.Sqrt( 2 ) * PICKER_SIZE / 2 + 2;
				AutoSizeAxes = Axes.Both;
				AddInternal( continuousHue = new Sprite().Center() );
				{
					if ( continuousTexture is null ) {
						continuousHue.Texture = continuousTexture = renderer.CreateTexture( (int)( radius + width + 4 ) * 2 * scale, (int)( radius + width + 4 ) * 2 * scale );
						continuousHue.Texture.SetData( TextureGeneration.HSVCircle( (int)( radius + width + 4 ) * 2 * scale, radius * scale, ( radius + width ) * scale ) );
					}
					else continuousHue.Texture = continuousTexture;

					continuousHue.Size = new Vector2( ( radius + width + 4 ) * 2 );
				}
				AddInternal( discreteHue = new Sprite().Center() );
				{
					radius -= width + 1;
					width += 1;
					if ( discreteTexture is null ) {
						discreteHue.Texture = discreteTexture = renderer.CreateTexture( (int)( radius + width + 4 ) * 2 * scale, (int)( radius + width + 4 ) * 2 * scale );
						discreteHue.Texture.SetData( TextureGeneration.HSVCircle( (int)( radius + width + 4 ) * 2 * scale, radius * scale, ( radius + width ) * scale, value: 0.7f, saturation: 0.9f, discreteCount: DISCRETE_COUNT ) );
					}
					else discreteHue.Texture = discreteTexture;

					discreteHue.Size = new Vector2( ( radius + width + 4 ) * 2 );
				}
				AddInternal( notch = new HueNotch {
					continuousOuterRadius = continuousHue.Size.X,
					continuousInnerRadius = discreteHue.Size.X,
					discreteOuterRadius = discreteHue.Size.X,
					discreteInnerRadius = discreteHue.Size.X - width,
					BypassAutoSizeAxes = Axes.Both
				}.Center() );
			}

			Sprite continuousHue;
			Sprite discreteHue;
			HueNotch notch;

			protected override void LoadComplete () {
				base.LoadComplete();
				LockedBindable.BindValueChanged( v => {
					this.FadeColour( v.NewValue ? Colour4.Gray : Colour4.White, 70 );
				}, true );

				HueBindable.BindValueChanged( v => {
					notch.RotateTo( v.NewValue );
				}, true );
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
			private void updateAt ( Vector2 pos ) {
				var x = pos.X - Size.X / 2;
				var y = Size.Y / 2 - pos.Y;

				var angle = MathF.Atan2( y, x ) / MathF.PI * 180;

				var distance = MathF.Sqrt( x * x + y * y ) * 2;
				notch.IsContinuous.Value = distance > notch.continuousInnerRadius;

				var prev = HueBindable.Value;
				HueBindable.Value = notch.IsContinuous.Value ? angle : ( MathF.Round( angle / ( 360 / DISCRETE_COUNT ) ) * ( 360 / DISCRETE_COUNT ) );

				if ( prev != HueBindable.Value && sampleTimer > sampleDelay ) {
					notchSample.Play();
					sampleTimer = 0;
				}
			}

			protected override void Update () {
				base.Update();
				sampleTimer += Time.Elapsed;
			}

			private class HueNotch : CircularProgress {
				const double CONTINUOUS_FILL = 16;

				public BindableBool IsContinuous = new( true );
				private double fill => IsContinuous.Value ? CONTINUOUS_FILL : ( 360 / DISCRETE_COUNT );

				public float continuousOuterRadius;
				public float discreteOuterRadius;
				public float continuousInnerRadius;
				public float discreteInnerRadius;
				private float outerRadius => IsContinuous.Value ? continuousOuterRadius : discreteOuterRadius;
				private float innerRadius => IsContinuous.Value ? continuousInnerRadius : discreteInnerRadius;

				protected override void LoadComplete () {
					IsContinuous.BindValueChanged( v => {
						this.FillTo( fill / 360, 100, Easing.Out );
						this.ResizeTo( outerRadius, 100, Easing.Out );
						this.TransformTo( nameof( InnerRadius ), 1 - (innerRadius / outerRadius), 100, Easing.Out );
					}, true );

					FinishTransforms();
				}

				public void RotateTo ( float hueDeg ) {
					var goalHue = 90 - hueDeg;
					var hueDistance = ( goalHue - Rotation ) % 360;
					if ( hueDistance < 0 ) hueDistance += 360;
					if ( hueDistance > 180 ) hueDistance -= 360;

					this.RotateTo( Rotation + hueDistance - (float)fill / 2, 200, Easing.Out );
				}
			}
		}
	}
}
