using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public abstract class Orbital : Container {
		public Trail Trail;

		public AnimatedDouble Distance;

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

		new IHasTilePosition Parent;

		public Orbital ( IHasTilePosition parent ) {
			AddInternal( Trail = new Trail() );

			Distance = new AnimatedDouble( parent: this );

			Parent = parent;
		}

		public void Hold () {
			Distance.Value = 0;
		}

		public void Release ( double? distance = null ) {
			Distance.Value = distance ?? DrawableTapTile.SPACING;
		}

		public void AnimateDistance ( double distance, double duration, Easing easing ) {
			Distance.AnimateTo( distance, duration, easing );
		}

		public Vector2 RotationVector
			=> new Vector2( (float)Math.Cos( Angle ), (float)Math.Sin( Angle ) );

		private double trailTimer = double.NaN;
		/// <summary>
		/// Trail duration in seconds
		/// </summary>
		private double traiDuration = 0.7;
		protected override void Update () {
			if ( double.IsNaN( trailTimer ) ) trailTimer = Clock.CurrentTime; // initial time

			Angle = Velocity * ( Clock.CurrentTime - startTime ) + startAngle;
			Position = RotationVector * (float)Distance;

			Trail.Offset = Parent.TilePosition + Position;
			if ( Clock.CurrentTime > trailTimer ) {
				double trailMSPV = traiDuration * 1000 / Trail.VerticeCount;
				while ( trailTimer + trailMSPV < Clock.CurrentTime ) {
					Trail.AddVertice( Trail.Offset );
					trailTimer += trailMSPV;
				}
			}
		}
	}
}
