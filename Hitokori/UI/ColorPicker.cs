using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Scoring;
using osuTK.Graphics;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class ColorPicker<T> : SettingsItem<Color4> where T : ColorPickerControl, new() {
		public new ColorPickerControl Control => (ColorPickerControl)base.Control;
		protected override Drawable CreateControl () {
			return new T();
		}

		public ColorPicker () {
			FlowContent.Direction = FillDirection.Vertical;
		}
	}

	public class ColorPicker : ColorPicker<ColorPickerControl> { }

	public class ColorPickerControl : Container, IHasCurrentValue<Color4> {
		protected readonly BindableWithCurrent<Color4> current = new BindableWithCurrent<Color4>();
		public Bindable<Color4> Current {
			get => current.Current;
			set => current.Current = value;
		}

		Sprite SVBox;
		Sprite HueRing;

		Circle previewColour;
		BlurableOsuTextBox hex;

		Container rightSide;
		protected Drawable Preview { get; private set; }
		Drawable previewBox;

		HueNotch hueSelector;
		SVNotch SVSelector;

		protected virtual Drawable CreatePreview () => null;

		const float size = 100;
		const float delta = 14;
		const float fade = 1;
		public ColorPickerControl () {
			Margin = new MarginPadding { Top = 5 };
			AutoSizeAxes = Axes.Y;
			RelativeSizeAxes = Axes.X;

			float radius = MathF.Sqrt( 2 ) * size;

			Child = new FillFlowContainer {
				AutoSizeAxes = Axes.Y,
				RelativeSizeAxes = Axes.X,
				Direction = FillDirection.Horizontal,
				Children = new Drawable[] {
					new Container {
						AutoSizeAxes = Axes.Both,
						Children = new Drawable[] {
							HueRing = new Sprite {
								Size = new osuTK.Vector2( (float)(radius + (delta+fade) * 2) ),
								Texture = new Texture((int)(radius + (delta+fade) * 2),(int)(radius + (delta+fade) * 2))
							}.Center(),
							new Container {
								Size = new osuTK.Vector2( (float)size ),
								Children = new Drawable[] {
									SVBox = new Sprite {
										RelativeSizeAxes = Axes.Both,
										Texture = new Texture( (int)size, (int)size )
									}.Center(),
									SVSelector = new SVNotch {
										HueProgress = hueProgress,
										SaturationProgress = saturationProgress,
										ValueProgress = valueProgress,

										OnValueChange = v => {
											this.TransformBindableTo( valueProgress, v, 200, Easing.Out );
										},
										OnSaturationChange = v => {
											this.TransformBindableTo( saturationProgress, v, 200, Easing.Out );
										}
									}
								}
							}.Center(),
							hueSelector = new HueNotch( HueRing ) {
								HueBindable = hueProgress,
								OnHueChange = v => {
									var goalHue = v;
									var hueDistance = (goalHue - hueProgress.Value) % 360;
									if ( hueDistance < 0 ) hueDistance += 360;
									if ( hueDistance > 180 ) hueDistance -= 360;

									this.TransformBindableTo( hueProgress, hueProgress.Value + hueDistance, 200, Easing.Out );
								}
							}.Center()
						}
					},
					rightSide = new Container {
						RelativeSizeAxes = Axes.Y,
						Margin = new MarginPadding { Left = 5 },
						Children = new Drawable[] {
							new FillFlowContainer {
								Direction = FillDirection.Horizontal,
								RelativeSizeAxes = Axes.X,
								AutoSizeAxes = Axes.Y,
								Children = new Drawable[] {
									previewColour = new Circle { Size = new osuTK.Vector2( 20 ), Margin = new MarginPadding { Right = 5 }, Anchor = Anchor.CentreLeft, Origin = Anchor.CentreLeft },
									hex = new BlurableOsuTextBox() { Width = 100, Height = 25, Margin = new MarginPadding(), Anchor = Anchor.CentreLeft, Origin = Anchor.CentreLeft },
								}
							}
						}
					}
				}
			};

			if ( ( Preview = CreatePreview() ) != null ) {
				rightSide.Add(
					previewBox = new Container {
						RelativeSizeAxes = Axes.X,
						Anchor = Anchor.BottomLeft,
						Origin = Anchor.BottomLeft,
						Masking = true,
						CornerRadius = 4,
						Children = new Drawable[] {
							new Box { RelativeSizeAxes = Axes.Both, Colour = new Color4( 0.1f, 0.1f, 0.1f, 1 ) },
							Preview
						}
					}
				);
			}

			HueRing.Texture.SetData( TextureGeneration.HSVCircle( (int)(radius + ( delta + fade ) * 2 ), radius / 2, (float)( radius / 2 + delta ), fadeDistance: (float)fade ) );
			SVBox.Texture.SetData( TextureGeneration.HSVBoxWithSetHue( (int)size, (int)size, 0 ) );

			current.BindValueChanged( v => {
				previewColour.Colour = 
				SVSelector.Colour	 = v.NewValue;
				if ( !textEdited ) hex.Blur();
				if ( !hex.HasFocus ) {
					hex.Text = v.NewValue.ToHex();
				}

				if ( !ignoreUpdate ) {
					var goal = TextureGeneration.RGBToHSV( new osuTK.Vector3( v.NewValue.R, v.NewValue.G, v.NewValue.B ) );

					// angle distance - bounded to (-180;180>
					var goalHue = goal.X;
					var hueDistance = ( goalHue - hueProgress.Value ) % 360;
					if ( hueDistance < 0 ) hueDistance += 360;
					if ( hueDistance > 180 ) hueDistance -= 360;

					this.TransformBindableTo( hueProgress, hueProgress.Value + hueDistance, 200, Easing.Out );
					this.TransformBindableTo( saturationProgress, goal.Y, 200, Easing.Out );
					this.TransformBindableTo( valueProgress, goal.Z, 200, Easing.Out );
				}
			}, true );

			hueProgress.BindValueChanged( v => {
				SVBox.Texture.SetData( TextureGeneration.HSVBoxWithSetHue( (int)size, (int)size, v.NewValue ) );
				setCurrent();
			}, true );

			saturationProgress.ValueChanged += _ => setCurrent();
			valueProgress.ValueChanged += _ => setCurrent();
		}

		string lastHex = "";
		bool ignoreUpdate;
		void setCurrent () {
			ignoreUpdate = true;
			var ch = TextureGeneration.FromHSV( hueProgress.Value, saturationProgress.Value, valueProgress.Value );
			Current.Value = new Color4( ch.R, ch.G, ch.B, 255 );
			ignoreUpdate = false;
		}

		BindableFloat hueProgress = new();
		BindableFloat saturationProgress = new();
		BindableFloat valueProgress = new();

		bool textEdited = false;
		protected override void Update () {
			base.Update();

			rightSide.Width = DrawWidth - 180;
			if ( previewBox is not null ) previewBox.Height = DrawHeight - 30;
			if ( hex.Text != lastHex ) {
				lastHex = hex.Text;

				if ( hex.HasFocus ) {
					var v = lastHex.TrimStart( '#' );
					if ( v.Length == 6 && v.All( c => "0123456789aAbBcCdDeEfF".Contains( c ) ) ) {
						var r = byte.Parse( v.Substring( 0, 2 ), System.Globalization.NumberStyles.HexNumber );
						var g = byte.Parse( v.Substring( 2, 2 ), System.Globalization.NumberStyles.HexNumber );
						var b = byte.Parse( v.Substring( 4, 2 ), System.Globalization.NumberStyles.HexNumber );
						textEdited = true;
						current.Value = new Color4( r, g, b, 255 );
						textEdited = false;
					}
				}
			}
		}

		private class HueNotch : CompositeDrawable {
			Drawable ring;
			public Action<float> OnHueChange;

			public IBindable<float> HueBindable { get; init; }
			public HueNotch ( Drawable ring ) {
				InternalChild = new Box { RelativeSizeAxes = Axes.Both, AlwaysPresent = true };
				Masking = true;
				BorderThickness = 2;
				BorderColour = Color4.Black;

				Size = new osuTK.Vector2( 12, delta + 12 );
				CornerRadius = 5;
				BypassAutoSizeAxes = Axes.Both;
				this.ring = ring;
			}

			protected override bool OnDragStart ( DragStartEvent e ) {
				return true;
			}

			protected override void OnDrag ( DragEvent e ) {
				var pos = Parent.ToSpaceOfOtherDrawable( e.MousePosition, ring );
				var angle = Math.Atan2( ring.DrawHeight / 2 - pos.Y, pos.X - ring.DrawWidth / 2 ) / Math.PI * 180;
				OnHueChange?.Invoke( (float)angle );
			}

			protected override void LoadComplete () {
				base.LoadComplete();

				HueBindable.BindValueChanged( v => {
					var c = TextureGeneration.FromHSV( v.NewValue, 1, 1 );
					InternalChild.Colour = new Colour4( c.R, c.G, c.B, 255 );
					Position = new osuTK.Vector2( MathF.Cos( v.NewValue * MathF.PI / 180 ), -MathF.Sin( v.NewValue * MathF.PI / 180 ) ) * ( MathF.Sqrt( 2 ) * size / 2 + delta / 2 );
					Rotation = 90 - v.NewValue;
				}, true );
			}
		}

		public class BlurableOsuTextBox : OsuTextBox {
			public void Blur () {
				if ( IsLoaded ) KillFocus();
			}
		}

		private class SVNotch : Circle {
			public IBindable<float> HueProgress { get; init; }
			public IBindable<float> SaturationProgress { get; init; }
			public IBindable<float> ValueProgress { get; init; }

			public SVNotch () {
				Size = new osuTK.Vector2( 15 );
				Origin = Anchor.Centre;
				RelativePositionAxes = Axes.Both;
				BorderThickness = 2;
				BorderColour = Color4.Black;
			}

			protected override void LoadComplete () {
				base.LoadComplete();

				HueProgress.ValueChanged += _ => updateColor();
				SaturationProgress.ValueChanged += _ => updateColor();
				ValueProgress.ValueChanged += _ => updateColor();

				updateColor();
			}

			private void updateColor () {
				var ch = TextureGeneration.FromHSV( HueProgress.Value, SaturationProgress.Value, ValueProgress.Value );
				Colour = new Color4( ch.R, ch.G, ch.B, 255 );

				Position = new osuTK.Vector2( SaturationProgress.Value, 1 - ValueProgress.Value );
			}

			protected override bool OnDragStart ( DragStartEvent e ) {
				return true;
			}

			public Action<float> OnSaturationChange;
			public Action<float> OnValueChange;
			protected override void OnDrag ( DragEvent e ) {
				var pos = e.MousePosition;
				OnSaturationChange?.Invoke( Math.Clamp( pos.X / Parent.DrawWidth, 0, 1 ) );
				OnValueChange?.Invoke( Math.Clamp( 1 - pos.Y / Parent.DrawHeight, 0, 1 ) );
			}
		}
	}
}
