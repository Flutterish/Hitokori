using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Screens.Edit.Setup;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Edit.Setup {
	public abstract class SetupSubsection : SetupSection {
		new public const float LABEL_WIDTH = SetupSection.LABEL_WIDTH;

		[MaybeNull, NotNull]
		Drawable side;

		[BackgroundDependencyLoader]
		private void load ( OverlayColourProvider colourProvider ) {
			Padding = Padding with { Horizontal = 20, Top = 5 };
			AddInternal( side = new GridContainer {
				RowDimensions = new Dimension[] {
					new Dimension( GridSizeMode.Absolute, 14 ),
					new Dimension( GridSizeMode.Absolute, 10 )
				},
				RelativeSizeAxes = Axes.Y,
				Width = 5,
				Position = new Vector2( -20, 0 ),

				Content = new Drawable[][] {
					new Drawable[] {
						new OsuClickableContainer {
							Width = 14,
							RelativeSizeAxes = Axes.Y,
							Anchor = Anchor.Centre,
							Origin = Anchor.Centre,
							Child = new ChevronToggle( IsExpandedBindable ) {
								RelativeSizeAxes = Axes.Both,
								FillMode = FillMode.Fill,
								Anchor = Anchor.Centre
							}
						}
					},
					new Drawable[] { },
					new Drawable[] {
						new Circle {
							Colour = colourProvider.Highlight1,
							RelativeSizeAxes = Axes.Both
						}
					}
				}
			} );

			ShowSideBindable.BindValueChanged( v => {
				side.FadeTo( v.NewValue ? 1 : 0, 200 );
			}, true );
			side.FinishTransforms();

			IsExpandedBindable.BindValueChanged( v => {
				if ( v.NewValue ) {
					Content.ScaleTo( new Vector2( 1, 1 ), 200, Easing.Out );
				}
				else {
					Content.ScaleTo( new Vector2( 1, 0 ), 200, Easing.In );
				}
			}, true );
		}

		public BindableBool ShowSideBindable = new( true );
		public bool ShowSide {
			get => ShowSideBindable.Value;
			set => ShowSideBindable.Value = value;
		}

		public readonly BindableBool IsExpandedBindable = new BindableBool( true );
		public bool IsExpanded {
			get => IsExpandedBindable.Value;
			set => IsExpandedBindable.Value = value;
		}

		private class ChevronToggle : SpriteIcon {
			BindableBool isExpanded = new BindableBool( true );
			public ChevronToggle ( BindableBool expandedBindable ) {
				Icon = FontAwesome.Solid.ChevronDown;
				Origin = Anchor.Centre;

				isExpanded.BindTo( expandedBindable );
				isExpanded.BindValueChanged( v => {
					if ( v.NewValue ) {
						this.RotateTo( 0, 200 );
					}
					else {
						this.RotateTo( -90, 200 );
					}
				}, true );
			}

			protected override bool OnHover ( HoverEvent e ) {
				this.ScaleTo( 1.4f, 100 );

				return true;
			}

			protected override void OnHoverLost ( HoverLostEvent e ) {
				this.ScaleTo( 1, 100 );
			}

			protected override bool OnClick ( ClickEvent e ) {
				isExpanded.Toggle();
				return true;
			}
		}
	}
}
