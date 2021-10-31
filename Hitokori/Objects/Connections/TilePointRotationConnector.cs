using osu.Game.Rulesets.Hitokori.ConstrainableProperties;
using osu.Game.Rulesets.Hitokori.Edit.Connectors;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
using osu.Game.Rulesets.Hitokori.Orbitals;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects {
	/// <summary>
	/// A connection between 2 <see cref="TilePoint"/>s where the orbital rotates around the <see cref="OrbitalState.PivotPosition"/> tile until it reaches the <see cref="TilePointConnector.To"/> tile.
	/// </summary>
	public class TilePointRotationConnector : TilePointConnector, IHasVelocity {
		/// <summary>
		/// The normalized distance from the pivot
		/// </summary>
		[Inspectable( Section = InspectableAttribute.SectionProperties, FormatMethod = nameof( FormatDistance ), ParseMethod = nameof( ParseDistance ) )]
		public readonly ConstrainableProperty<double> Radius;
		/// <summary>
		/// The signed angle change in radians
		/// </summary>
		[Inspectable( Section = InspectableAttribute.SectionProperties, FormatMethod = nameof( FormatAngleRadiansToDegrees ), ParseMethod = nameof( ParseDegreeAngleToRadians ) )]
		public readonly ConstrainableAngle Angle;
		/// <summary>
		/// Signed speed in arclength per ms. This is essentially angle in radians per ms at <see cref="Radius"/> = 1
		/// </summary>
		[Inspectable( Section = InspectableAttribute.SectionProperties, FormatMethod = nameof( FormatVelocity ), ParseMethod = nameof( ParseVelocity ) )]
		public readonly ConstrainableProperty<double> Velocity;
		double IHasVelocity.Velocity => Velocity;

		public TilePointRotationConnector () {
			Radius = new( 1, recalculate, onConstraintChanged );// { Unit = "Tile", UnitPlural = "Tiles" };
			Angle = new ConstrainableAngle( recalculate, onConstraintChanged ) { IsRadians = true };
			Velocity = new( recalculate, onConstraintChanged );// { Unit = "Tile per second", UnitPlural = "Tiles per second", CustomFormat = x => 1000 * x };
		}

		private void onConstraintChanged ( bool isConstrained )
			=> Invalidate();

		/// <summary>
		/// Unsigned speed in arclength per ms. This is essentially angle in radians per ms at <see cref="Radius"/> = 1. To set this value you need to set <see cref="Velocity"/>.
		/// </summary>
		public double Speed => Math.Abs( Velocity );

		private double distancePerBeat = 120d / 180 * Math.PI;
		/// <summary>
		/// Distance in arclength per beat. This is essentially angle in radians per beat at <see cref="Radius"/> = 1.
		/// </summary>
		[Inspectable( Section = InspectableAttribute.SectionProperties, Label = "Distance per beat", FormatMethod = nameof( FormatDistance ), ParseMethod = nameof( ParseDistance ) )]
		public double DistancePerBeat {
			get => distancePerBeat;
			set {
				distancePerBeat = value;
				Invalidate();
			}
		}

		private const double maxAngle = Math.PI * 1.75;
		private const double minAngle = Math.PI * 0;

		protected override void InvalidateProperties () {
			base.InvalidateProperties();

			if ( Radius.IsConstrained || Angle.IsConstrained || Velocity.IsConstrained ) {
				throw new NotImplementedException( "Respecting constraints is not implemented yet" );
			}

			Radius.Invalidate();
			Angle.Invalidate();
			Velocity.Invalidate();
		}

		void recalculate () {
			if ( Radius.IsConstrained || Angle.IsConstrained || Velocity.IsConstrained ) {
				throw new NotImplementedException( "Respecting constraints is not implemented yet" ); // TODO constriants
			}
			else {
				var distance = distancePerBeat * Beats;
				Radius.Value = From.OrbitalState.UnscaledOffsetOfNth( TargetOrbitalIndex ).Length;
				if ( Radius != 0 ) {
					// TODO This could be calculated with a spiral for a better approximation
					Angle.Value = distance / Radius;

					if ( Angle > maxAngle ) {
						Angle.Value = maxAngle;
						Radius.Value = Math.Min( distance / maxAngle, Radius * 2 );
					}

					Angle.Value = Math.Clamp( distance / Radius, minAngle, maxAngle );
				}
				else {
					Angle.Value = 0;
				}

				Angle.Value *= TargetOrbitalIndex > 0 ? 1 : -1;

				if ( Duration != 0 ) {
					Velocity.Value = ( Angle * Radius ) / Duration;
				}
				else {
					Velocity.Value = TargetOrbitalIndex > 0 ? double.PositiveInfinity : double.NegativeInfinity;
				}
			}
		}

		double targetScale {
			get {
				var unscaled = From.OrbitalState.UnscaledOffsetOfNth( TargetOrbitalIndex ).Length;

				if ( unscaled != 0 )
					return Radius / unscaled;
				else
					return 1;
			}
		}

		public override OrbitalState GetStateAt ( double progress )
			=> From.OrbitalState.WithScale( From.OrbitalState.Scale + ( targetScale - From.OrbitalState.Scale ) * Math.Clamp( progress, 0, 1 ) ).RotatedBy( Angle * progress ).WithOffset(
				osuTK.Vector2d.One / 80 * progress
			);
	}
}
