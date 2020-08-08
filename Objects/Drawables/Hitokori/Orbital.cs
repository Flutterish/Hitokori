using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori { // TODO add sparkels when held
	public abstract class Orbital : Container {
		public Trail Trail;

		Radius Radius;
		bool isCentered = false;
		public double Distance => isCentered ? 0 : Radius.Length;

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
					Velocity = -Velocity;
					InterpolateTrailTo( Clock.CurrentTime - deltaTime );
					trailTimer = Clock.CurrentTime;
					Velocity = -Velocity;
				} else {
					InterpolateTrailTo( Clock.CurrentTime + deltaTime );
				}
			}
			Angle = angle;
		}

		new IHasTilePosition Parent;

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
					Position = RotationVector * (float)Distance;

					Trail.Offset = Parent.TilePosition + Position;
					Trail.AddVertice( Trail.Offset );
				}
			} else {
				// if time goes backward we dont want the trail to stop
				trailTimer = time;
			}

			Angle = Velocity * ( Clock.CurrentTime - startTime ) + startAngle;
			Position = RotationVector * (float)Distance;

			Trail.Offset = Parent.TilePosition + Position;
		}

		public abstract void MakeImportant ();
		public abstract void RevokeImportant ();
	}
}
