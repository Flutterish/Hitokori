using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using osuTK.Graphics;
using Sentry;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.UI {
	public partial class ColorPickerControl : FillFlowContainer, IHasCurrentValue<Color4> {
		protected Drawable Preview { get; private set; }
		protected virtual Drawable CreatePreview () => null;

		protected readonly BindableWithCurrent<Color4> current = new BindableWithCurrent<Color4>();
		public Bindable<Color4> Current {
			get => current.Current;
			set => current.Current = value;
		}

		BindableHSVColor HSV = new();
		Bindable<bool> lockedBindable = new( true );

		public ColorPickerControl () {
			HSV.Color.BindTo( current );

			AutoSizeAxes = Axes.Y;
			RelativeSizeAxes = Axes.X;
			Direction = FillDirection.Horizontal;

			HSV.Color.BindValueChanged( v => {
				Current.Value = v.NewValue;
				colorPreview.FadeColour( v.NewValue, 100 );
				if ( !hexPreview.HasFocus ) hexPreview.Text = v.NewValue.ToHex();
			} );

			Children = new Drawable[] {
				pickerSide = new Container {
					AutoSizeAxes = Axes.Both,
					Children = new Drawable[] {
						huePicker = new HuePicker {
							Anchor = Anchor.Centre,
							Origin = Anchor.Centre,

							HueBindable = HSV.H,
							LockedBindable = lockedBindable
						},
						svPicker = new SVPicker {
							Anchor = Anchor.Centre,
							Origin = Anchor.Centre,

							HueBindable = HSV.H,
							SaturationBindable = HSV.S,
							ValueBindable = HSV.V,
							LockedBindable = lockedBindable,
							ColorBindable = HSV.Color
						}
					}
				},
				colorLockAndPreviewSide = new FillFlowContainer {
					RelativeSizeAxes = Axes.Y,
					Direction = FillDirection.Vertical,
					Children = new Drawable[] {
						colorAndLockSide = new FillFlowContainer {
							Direction = FillDirection.Vertical,
							AutoSizeAxes = Axes.Y,
							RelativeSizeAxes = Axes.X,
							Children = new Drawable[] {
								new FillFlowContainer {
									Direction = FillDirection.Horizontal,
									RelativeSizeAxes = Axes.X,
									Height = COLOR_HEIGHT,
									Children = new Drawable[] {
										colorPreview = new Circle {
											Size = new Vector2( COLOR_HEIGHT * 0.8f ),
											Origin = Anchor.CentreLeft,
											Anchor = Anchor.CentreLeft,
										},
										hexPreview = new OsuTextBox {
											Height = COLOR_HEIGHT * 0.8f,
											Width = 70,
											Origin = Anchor.CentreLeft,
											Anchor = Anchor.CentreLeft,
											Margin = new MarginPadding { Horizontal = 5 }
										}
									}
								},
								lockToggle = new OsuCheckbox {
									LabelText = "Lock",
									RelativeSizeAxes = Axes.X,
									AutoSizeAxes = Axes.Y,
									Current = lockedBindable
								}
							}
						},
						previewSide = new Container {
							RelativeSizeAxes = Axes.X
						}
					}
				}
			};

			if ( ( Preview = CreatePreview() ) is not null ) {
				previewSide.Add(
					new Container {
						RelativeSizeAxes = Axes.Both,
						Masking = true,
						CornerRadius = 6,
						Children = new Drawable[] {
							new Box { RelativeSizeAxes = Axes.Both, Colour = new Color4( 0.1f, 0.1f, 0.1f, 1 ) },
							Preview
						}
					}
				);
			}

			hexPreview.Current.ValueChanged += v => {
				if ( !hexPreview.HasFocus ) return;

				var text = hexPreview.Text.TrimStart( '#' );
				if ( text.Length == 6 && text.All( c => "0123456789aAbBcCdDeEfF".Contains( c ) ) ) {
					var r = byte.Parse( text.Substring( 0, 2 ), System.Globalization.NumberStyles.HexNumber );
					var g = byte.Parse( text.Substring( 2, 2 ), System.Globalization.NumberStyles.HexNumber );
					var b = byte.Parse( text.Substring( 4, 2 ), System.Globalization.NumberStyles.HexNumber );
					HSV.Color.Value = new Color4( r, g, b, 255 );
				}
			};

			lockedBindable.BindValueChanged( v => {
				hexPreview.ReadOnly = v.NewValue;
			}, true );
		}

		const float COLOR_HEIGHT = 35;
		const float PICKER_SIZE = 110;

		Container pickerSide;
		FillFlowContainer colorLockAndPreviewSide;
		FillFlowContainer colorAndLockSide;
		Container previewSide;

		Circle colorPreview;
		OsuTextBox hexPreview;
		OsuCheckbox lockToggle;
		SVPicker svPicker;
		HuePicker huePicker;

		protected override void Update () {
			base.Update();

			colorLockAndPreviewSide.Width = DrawWidth - pickerSide.DrawWidth;
			previewSide.Height = colorLockAndPreviewSide.DrawHeight - lockToggle.DrawHeight - COLOR_HEIGHT;
			hexPreview.Width = colorAndLockSide.DrawWidth - colorPreview.DrawWidth - 10;
		}
	}
}
