using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
    public abstract class Orbital : Container
    {
		public Trail Trail;

		protected Radius Radius;
		bool isCentered = false;
		public double Distance => isCentered ? 0 : Radius.Length;
		private Vector2 targetTilePosition => Parent.TilePosition + RotationVector * (float)Distance;
		public Vector2 TilePosition {
			get {
				animationCurve.End = targetTilePosition;
				return animationCurve.Evaluate( (float)animationProgress.Value );
			}
		}

		/// <summary>
		/// Velocity in radians per millisecond
		/// </summary>
		public double Velocity {
			get => velocity;
			set {
				startTime = Clock.CurrentTime;
				velocity = value;
				startAngle = Angle;
			}
		}
		private double startTime;
		private double velocity;
		private double startAngle;

		public double Angle;

		public void RotateTo ( double angle ) {
			double deltaTime = ( angle - Angle ) / Velocity;
			if ( double.IsFinite( deltaTime ) ) {
				if ( deltaTime < 0 ) {
					trailTimer = Clock.CurrentTime;
				}
				else {
					InterpolateTrailTo( Clock.CurrentTime + deltaTime );
				}
			}
			Angle = angle;
		}

		new IHasTilePosition Parent;
		private QuadraticBezier animationCurve;
		private Bindable<double> animationProgress = new( 1 );
		public void AnimateLate ( double duration ) {
			animationCurve.Start = TilePosition;
			animationCurve.Control = animationCurve.Start + VectorVelocity * 4000;
			animationProgress.Value = 0;
			this.TransformBindableTo( animationProgress, 1, duration, Easing.In );
		}
		public void AnimateEarly ( double duration ) {
			animationCurve.Start = TilePosition;
			animationCurve.Control = animationCurve.Start;
			animationProgress.Value = 0;
			this.TransformBindableTo( animationProgress, 1, duration, Easing.InBounce );
		}

		public Vector2 VectorVelocity => (float)Velocity * (float)( Distance / HitokoriTile.SPACING ) * RotationVector.PerpendicularLeft;

		public Orbital ( IHasTilePosition parent, Radius radius ) {
			AddInternal( Trail = new Trail() );
			Radius = radius;

			Parent = parent;
		}

		public void Hold () {
			isCentered = true;
		}

		public void Release () {
			isCentered = false;
		}

		public Vector2 RotationVector
			=> new Vector2( (float)Math.Cos( Angle ), (float)Math.Sin( Angle ) );

		private double trailTimer;
		/// <summary>
		/// Trail duration in seconds
		/// </summary>
		private double traiDuration = ( 1.0 / 240 ) * 100; // old behaviour is 100 update frames total, i was running on abt 240 of these
		protected override void Update () {
			InterpolateTrailTo( Clock.CurrentTime );
		}

		private void InterpolateTrailTo ( double time ) {
			if ( time > trailTimer ) {
				double trailMSPV = traiDuration * 1000 / Trail.VerticeCount;
				while ( trailTimer + trailMSPV < time ) {
					trailTimer += trailMSPV; // interpolating the position
					Angle = Velocity * ( trailTimer - startTime ) + startAngle;

					Position = TilePosition - Parent.TilePosition;
					Trail.Offset = Parent.TilePosition + Position;

					Trail.AddVertice( Trail.Offset );
				}
			}
			else {
				// if time goes backward we dont want the trail to stop
				trailTimer = time;
			}
			Angle = Velocity * ( Clock.CurrentTime - startTime ) + startAngle;

			Position = TilePosition - Parent.TilePosition;
			Trail.Offset = Parent.TilePosition + Position;
		}

		public abstract void MakeImportant ();
		public abstract void RevokeImportant ();

		public abstract void OnHold ();
		public abstract void OnRelease ();
		public abstract void OnPress ();
	}
}
