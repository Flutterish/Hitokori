using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.UI.Visuals {
	public class TilePointVisualPiece : CompositeDrawable {
		protected Drawable Body;
		protected Drawable LineIn;
		protected Drawable LineOut;
		protected Drawable BodyOutline;
		protected Drawable LineInOutline;
		protected Drawable LineOutOutline;
		protected Drawable BodyShadow;
		protected Drawable LineInShadow;
		protected Drawable LineOutShadow;
		protected Container Shadows;

		public TilePointVisualPiece () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AutoSizeAxes = Axes.Both;

			AddInternal( Shadows = new Container {
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = Vector2.Zero,
				Children = new Drawable[] {
					LineInShadow = new Box {
						Colour = Colour4.Black.MultiplyAlpha( 0.3f ),
						Anchor = Anchor.Centre,
						Origin = Anchor.CentreLeft,
						Height = 32
					},
					LineOutShadow = new Box {
						Colour = Colour4.Black.MultiplyAlpha( 0.3f ),
						Anchor = Anchor.Centre,
						Origin = Anchor.CentreLeft,
						Height = 32
					},
					BodyShadow = new Circle {
						Colour = Colour4.Black.MultiplyAlpha( 0.3f ),
						Anchor = Anchor.Centre,
						Origin = Anchor.Centre,
						Size = new Vector2( 32 )
					}
				}
			} );

			AddInternal( LineInOutline = new Box {
				Colour = Colour4.White,
				Anchor = Anchor.Centre,
				Origin = Anchor.CentreLeft,
				Height = 26
			} );
			AddInternal( LineOutOutline = new Box {
				Colour = Colour4.White,
				Anchor = Anchor.Centre,
				Origin = Anchor.CentreLeft,
				Height = 26
			} );
			AddInternal( BodyOutline = new Circle {
				Colour = Colour4.White,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 26 )
			} );

			AddInternal( LineIn = new Box {
				Colour = Colour4.HotPink,
				Anchor = Anchor.Centre,
				Origin = Anchor.CentreLeft,
				Height = 22
			} );
			AddInternal( LineOut = new Box {
				Colour = Colour4.HotPink,
				Anchor = Anchor.Centre,
				Origin = Anchor.CentreLeft,
				Height = 22
			} );
			AddInternal( Body = new Circle {
				Colour = Colour4.HotPink,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 22 )
			} );

			InAnimationProgress.BindValueChanged( _ => updateFromConnector() );
			OutAnimationProgress.BindValueChanged( _ => updateToConnector() );
			FromPosition.BindValueChanged( _ => updateFromConnector() );
			ToPosition.BindValueChanged( _ => updateToConnector() );
			PositionScale.BindValueChanged( _ => {
				updateFromConnector();
				updateToConnector();
			} );
			AroundPosition.BindValueChanged( _ => {
				updateFromConnector();
				updateToConnector();
			}, true );
		}

		public readonly BindableFloat InAnimationProgress = new();
		public readonly BindableFloat OutAnimationProgress = new();

		new public Color4 Colour {
			get => Body.Colour;
			set {
				Body.Colour = LineIn.Colour = LineOut.Colour = value;
			}
		}

		new public Color4 BorderColour {
			get => Body.Colour;
			set {
				BodyOutline.Colour = LineInOutline.Colour = LineOutOutline.Colour = value;
			}
		}

		private bool showShadows = true;
		public bool ShowShadows {
			get => showShadows;
			set {
				showShadows = value;
				if ( showShadows )
					Shadows.Show();
				else
					Shadows.Hide();
			}
		}

		public void LightUp () {
			var lightUpDuration = 180;
			var lightUpEasing = Easing.None;

			Shadows.FadeOut( lightUpDuration, lightUpEasing );

			Body.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
			LineIn.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
			LineOut.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
			BodyOutline.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
			LineInOutline.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
			LineOutOutline.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
		}

		public readonly BindableFloat PositionScale = new( HitokoriPlayfield.DefaultPositionScale );
		public readonly Bindable<Vector2d?> FromPosition = new( null );
		public readonly Bindable<Vector2d?> ToPosition = new( null );
		public readonly Bindable<Vector2d> AroundPosition = new();

		private bool overlapConnectors = true;
		public bool OverlapConnectors {
			get => overlapConnectors;
			set {
				overlapConnectors = value;
				updateFromConnector();
				updateToConnector();
			}
		}

		private void updateFromConnector () {
			if ( FromPosition.Value is null ) {
				LineIn.Alpha = 0;
				LineInOutline.Alpha = 0;
				LineInShadow.Alpha = 0;
			}
			else {
				LineIn.Alpha = 1;
				LineInOutline.Alpha = 1;
				LineInShadow.Alpha = 1;

				var around = AroundPosition.Value;
				var from = FromPosition.Value.Value;

				LineIn.Rotation = LineInOutline.Rotation = LineInShadow.Rotation = (float)around.AngleTo( from ).RadToDeg();

				var expansion = (float)( ( from - around ).Length * PositionScale.Value + ( overlapConnectors ? 2 : 0 ) ) / 2 * InAnimationProgress.Value;

				LineIn.Width = Math.Max( expansion - 2, 0 );
				LineInOutline.Width = Math.Max( expansion, 0 );
				LineInShadow.Width = Math.Max( expansion - 1, 0 );
			}
		}

		private void updateToConnector () {
			if ( ToPosition.Value is null ) {
				LineOut.Alpha = 0;
				LineOutOutline.Alpha = 0;
				LineOutShadow.Alpha = 0;
			}
			else {
				LineOut.Alpha = 1;
				LineOutOutline.Alpha = 1;
				LineOutShadow.Alpha = 1;

				var around = AroundPosition.Value;
				var to = ToPosition.Value.Value;

				LineOut.Rotation = LineOutOutline.Rotation = LineOutShadow.Rotation = (float)around.AngleTo( to ).RadToDeg();

				var expansion = (float)( ( to - around ).Length * PositionScale.Value + ( overlapConnectors ? 2 : 0 ) ) / 2 * OutAnimationProgress.Value;

				LineOut.Width = Math.Max( expansion - 2, 0 );
				LineOutOutline.Width = Math.Max( expansion, 0 );
				LineOutShadow.Width = Math.Max( expansion - 1, 0 );
			}
		}
	}
}
