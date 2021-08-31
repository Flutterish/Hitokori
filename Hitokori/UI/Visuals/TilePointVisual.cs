using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.UI.Visuals {
	public class TilePointVisual : AppliableVisual<TilePoint> {
		Drawable body;
		Drawable lineIn;
		Drawable lineOut;
		Drawable bodyOutline;
		Drawable lineInOutline;
		Drawable lineOutOutline;
		Drawable bodyShadow;
		Drawable lineInShadow;
		Drawable lineOutShadow;

		public TilePointVisual () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AddInternal( lineInShadow = new Box {
				Colour = Colour4.Black,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 16,
				Alpha = 0.3f
			} );
			AddInternal( lineOutShadow = new Box {
				Colour = Colour4.Black,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 16,
				Alpha = 0.3f
			} );
			AddInternal( bodyShadow = new Circle {
				Colour = Colour4.Black,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 30 ),
				Alpha = 0.3f,
				Position = new Vector2( 3 )
			} );

			AddInternal( lineInOutline = new Box {
				Colour = Colour4.White,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 16
			} );
			AddInternal( lineOutOutline = new Box {
				Colour = Colour4.White,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 16
			} );
			AddInternal( bodyOutline = new Circle {
				Colour = Colour4.White,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 30 )
			} );

			AddInternal( lineIn = new Box {
				Colour = Colour4.HotPink,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 12
			} );
			AddInternal( lineOut = new Box {
				Colour = Colour4.HotPink,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 12
			} );
			AddInternal( body = new Circle {
				Colour = Colour4.HotPink,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 26 )
			} );
		}

		BindableFloat inAnimationProgress = new();
		BindableFloat outAnimationProgress = new();

		public override void UpdateInitialTransforms () {
			if ( AppliedHitObject.ToNext is null ) {
				lineOut.Hide();
				lineOutOutline.Hide();
				lineOutShadow.Hide();
			}
			else {
				lineOut.Show();
				lineOutOutline.Show();
				lineOutShadow.Show();
				lineOutShadow.Alpha = 0.3f;
			}

			if ( AppliedHitObject.Previous is null ) {
				lineIn.Hide();
				lineInOutline.Hide();
				lineInShadow.Hide();
			}
			else {
				lineIn.Show();
				lineInOutline.Show();
				lineInShadow.Show();
				lineInShadow.Alpha = 0.3f;
			}

			using ( BeginAbsoluteSequence( AppliedHitObject.StartTime - 2000 ) ) {
				inAnimationProgress.Value = 0;
				this.TransformBindableTo( inAnimationProgress, 1, 750, Easing.Out );
			}
			using ( BeginAbsoluteSequence( AppliedHitObject.StartTime - 2000 ) ) {
				outAnimationProgress.Value = 0;
				this.TransformBindableTo( outAnimationProgress, 1, 750, Easing.Out );
			}
		}

		protected override void Update () {
			if ( !IsApplied ) return;

			if ( AppliedHitObject.FromPrevious is TilePointConnector ) {
				lineIn.Rotation = lineInOutline.Rotation = lineInShadow.Rotation = (float)AppliedHitObject.Position.AngleTo( AppliedHitObject.Previous.Position ).RadToDeg();
				lineIn.Width = (float)( AppliedHitObject.Previous.Position - AppliedHitObject.Position ).Length * 100 / 2 * inAnimationProgress.Value;
				lineInOutline.Width = lineInShadow.Width = lineIn.Width + 4;

				lineIn.Position = lineInOutline.Position = lineIn.Rotation.DegToRad().AngleToVector() * lineIn.Width / 2;
				lineInShadow.Position = lineInOutline.Position + Vector2.One * 3;
			}

			if ( AppliedHitObject.ToNext is TilePointConnector ) {
				lineOut.Rotation = lineOutOutline.Rotation = lineOutShadow.Rotation = (float)AppliedHitObject.Position.AngleTo( AppliedHitObject.Next.Position ).RadToDeg();
				lineOut.Width = (float)( AppliedHitObject.Next.Position - AppliedHitObject.Position ).Length * 100 / 2 * outAnimationProgress.Value;
				lineOutOutline.Width = lineOutShadow.Width = lineOut.Width + 4;

				lineOut.Position = lineOutOutline.Position = lineOut.Rotation.DegToRad().AngleToVector() * lineOut.Width / 2;
				lineOutShadow.Position = lineOutOutline.Position + Vector2.One * 3;
			}
		}
	}
}
