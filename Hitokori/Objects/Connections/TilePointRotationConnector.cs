#nullable enable

using osu.Game.Rulesets.Hitokori.Orbitals;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects {
	/// <summary>
	/// A connection between 2 <see cref="TilePoint"/>s where the orbital rotates around the <see cref="OrbitalState.PivotPosition"/> tile until it reaches the <see cref="TilePointConnector.To"/> tile.
	/// </summary>
	public class TilePointRotationConnector : TilePointConnector {
		public bool IsRadiusConstrained { get; private set; } = false;
		private bool isRadiusComputed = false;
		private double radius = 1;
		/// <summary>
		/// The normalized distance from the <see cref="Around"/>. Setting this property will contrain it to that value.
		/// </summary>
		public double Radius {
			get {
				if ( !IsRadiusConstrained && !isRadiusComputed ) recalculate();
				return radius;
			}
			set {
				IsRadiusConstrained = true;
				radius = value;
				Invalidate();
			}
		}

		public bool IsAngleConstrained { get; private set; } = false;
		private bool isAngleComputed = false;
		private double angle;
		/// <summary>
		/// The signed angle change in radians. Setting this property will contrain it to that value.
		/// </summary>
		public double Angle {
			get {
				if ( !IsAngleConstrained && !isAngleComputed ) recalculate();
				return angle;
			}
			set {
				IsAngleConstrained = true;
				angle = value;
				Invalidate();
			}
		}

		public bool IsVelocityConstrained { get; private set; } = false;
		private bool isVelocityComputed = false;
		private double velocity;
		/// <summary>
		/// Signed speed in arclength per ms. This is essentially angle in radians per ms at <see cref="Radius"/> = 1. Setting this property will contrain it to that value.
		/// </summary>
		public double Velocity {
			get {
				if ( !IsVelocityConstrained && !isVelocityComputed ) recalculate();
				return velocity;
			}
			set {
				IsVelocityConstrained = true;
				velocity = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Unsigned speed in arclength per ms. This is essentially angle in radians per ms at <see cref="Radius"/> = 1. To set this value you need to set <see cref="Velocity"/>.
		/// </summary>
		public double Speed => Math.Abs( Velocity );

		private double distancePerBeat = 120f / 180 * Math.PI;
		/// <summary>
		/// Distance in arclength per beat. This is essentially angle in radians per beat at <see cref="Radius"/> = 1.
		/// </summary>
		public double DistancePerBeat {
			get => distancePerBeat;
			set {
				distancePerBeat = value;
				Invalidate();
			}
		}

		private const double maxAngle = Math.PI * 1.75f;
		private const double minAngle = Math.PI * 0f;

		protected override void InvalidateProperties () {
			base.InvalidateProperties();

			isRadiusComputed = false;
			isAngleComputed = false;
			isVelocityComputed = false;
		}

		void recalculate () {
			if (IsRadiusConstrained || IsAngleConstrained || IsVelocityConstrained) {
				throw new NotImplementedException( "Respecting constraints is not implemented yet" ); // TODO constriants
			}

			var distance = distancePerBeat * Beats;
			radius = From.OrbitalState.OffsetOfNth( TargetOrbitalIndex ).Length;
			// TODO we are still travelling at angle/time instead of arclength/time. this here could be calculated with a spiral for a better approximation
			angle = Math.Clamp( distance / radius, minAngle, maxAngle );


			angle *= TargetOrbitalIndex > 0 ? 1 : -1;
			velocity = ( angle * radius ) / (float)Duration;

			isRadiusComputed = true;
			isAngleComputed = true;
			isVelocityComputed = true;
		}

		double targetScale => Radius / From.OrbitalState.OffsetOfNth( TargetOrbitalIndex ).Length;

		public override OrbitalState GetStateAt ( double progress )
			=> From.OrbitalState.WithScale( From.OrbitalState.Scale + ( targetScale - From.OrbitalState.Scale ) * Math.Clamp( progress, 0, 1 ) ).RotatedBy( Angle * progress );
	}
}
