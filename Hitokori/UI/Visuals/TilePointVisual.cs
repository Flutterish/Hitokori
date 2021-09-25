using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;
using System.Linq;

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
				Colour = Colour4.Black.MultiplyAlpha( 0.3f ),
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 32
			} );
			AddInternal( lineOutShadow = new Box {
				Colour = Colour4.Black.MultiplyAlpha( 0.3f ),
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 32
			} );
			AddInternal( bodyShadow = new Circle {
				Colour = Colour4.Black.MultiplyAlpha( 0.3f ),
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 32 )
			} );

			AddInternal( lineInOutline = new Box {
				Colour = Colour4.White,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 26
			} );
			AddInternal( lineOutOutline = new Box {
				Colour = Colour4.White,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 26
			} );
			AddInternal( bodyOutline = new Circle {
				Colour = Colour4.White,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 26.6f )
			} );

			AddInternal( lineIn = new Box {
				Colour = Colour4.HotPink,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 22
			} );
			AddInternal( lineOut = new Box {
				Colour = Colour4.HotPink,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Height = 22
			} );
			AddInternal( body = new Circle {
				Colour = Colour4.HotPink,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 22 )
			} );
		}

		BindableFloat inAnimationProgress = new();
		BindableFloat outAnimationProgress = new();

		new public Color4 Colour {
			get => body.Colour;
			set {
				body.Colour = lineIn.Colour = lineOut.Colour = value;
			}
		}

		protected override void OnApply ( TilePoint hitObject ) {
			base.OnApply( hitObject );

			if ( hitObject.FromPrevious is IHasVelocity fromv && hitObject.ToNext is IHasVelocity tov ) {
				if ( fromv.Speed / tov.Speed < 0.95 ) {
					Colour = Colour4.Tomato;
				}
				else if ( tov.Speed / fromv.Speed < 0.95 ) {
					Colour = Colour4.LightBlue;
				}
				else {
					Colour = Colour4.HotPink;
				}
			}
		}

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

				this.TransformBindableTo( outAnimationProgress, 0 );
				using ( BeginAbsoluteSequence( AppliedHitObject.Next.StartTime - 2000 ) ) {
					this.TransformBindableTo( outAnimationProgress, 1, 750, Easing.Out );
				}
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

				this.TransformBindableTo( inAnimationProgress, 0 );
				using ( BeginAbsoluteSequence( AppliedHitObject.StartTime - 2000 ) ) {
					this.TransformBindableTo( inAnimationProgress, 1, 750, Easing.Out );
				}
			}

			this.ScaleTo( 0 ).Then().ScaleTo( 1, 300, Easing.Out );
		}

		public override void UpdateHitStateTransforms ( ArmedState state ) {
			var lightUpDuration = 180;
			var lightUpEasing = Easing.None;
			var fadeOutDuration = 300;
			var fadeOutEasing = Easing.In;

			if ( state is ArmedState.Hit or ArmedState.Miss ) {
				lineInShadow.FadeOut( lightUpDuration, lightUpEasing );
				lineOutShadow.FadeOut( lightUpDuration, lightUpEasing );
				bodyShadow.FadeOut( lightUpDuration, lightUpEasing );

				body.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
				lineIn.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
				lineOut.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
				bodyOutline.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
				lineInOutline.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );
				lineOutOutline.FadeColour( Colour4.White, lightUpDuration, lightUpEasing );

				var nextSwapTile = AppliedHitObject.AllNext.FirstOrDefault( x => x.OrbitalState.ActiveIndex != AppliedHitObject.OrbitalState.ActiveIndex );
				using ( BeginAbsoluteSequence( nextSwapTile?.StartTime ?? LatestTransformEndTime ) ) {
					this.ScaleTo( 0, fadeOutDuration, fadeOutEasing );
				}
			}
		}

		private BindableFloat positionScale = new( HitokoriPlayfield.DefaultPositionScale );
		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );
		}

		protected override void Update () {
			if ( !IsApplied ) return;

			if ( AppliedHitObject.FromPrevious is not null ) {
				lineIn.Rotation = lineInOutline.Rotation = lineInShadow.Rotation = (float)AppliedHitObject.Position.AngleTo( AppliedHitObject.Previous.Position ).RadToDeg();
				lineIn.Width = (float)( AppliedHitObject.Previous.Position - AppliedHitObject.Position ).Length * positionScale.Value / 2 * inAnimationProgress.Value - 1;
				lineInOutline.Width = lineIn.Width + 4;
				lineInShadow.Width = lineIn.Width + 2;

				lineInShadow.Position = lineIn.Position = lineInOutline.Position = lineIn.Rotation.DegToRad().AngleToVector() * lineIn.Width / 2;
			}

			if ( AppliedHitObject.ToNext is not null ) {
				lineOut.Rotation = lineOutOutline.Rotation = lineOutShadow.Rotation = (float)AppliedHitObject.Position.AngleTo( AppliedHitObject.Next.Position ).RadToDeg();
				lineOut.Width = (float)( AppliedHitObject.Next.Position - AppliedHitObject.Position ).Length * positionScale.Value / 2 * outAnimationProgress.Value - 1;
				lineOutOutline.Width = lineOut.Width + 4;
				lineOutShadow.Width = lineOut.Width + 2;

				lineOutShadow.Position = lineOut.Position = lineOutOutline.Position = lineOut.Rotation.DegToRad().AngleToVector() * lineOut.Width / 2;
			}
		}
	}
}
