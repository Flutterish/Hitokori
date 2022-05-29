using osu.Game.Rulesets.Hitokori.ConstrainableProperties;
using osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
using osu.Game.Rulesets.Hitokori.Orbitals;

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
			Radius = new( 1, recalculate, onRadiusConstraintChanged );
			Angle = new ConstrainableAngle( recalculate, onAngleConstraintChanged ) { IsRadians = true };
			Velocity = new( recalculate, onVelocityConstraintChanged );

			TargetOrbitalIndex = 1;
		}

		private void onVelocityConstraintChanged ( bool isConstrained ) {
			var constraints = ensureValidConstraints();

			if ( constraints is ( Constraint.Angle | Constraint.Velocity ) ) {
				if ( Math.Sign( Velocity * Angle ) == -1 ) Angle.Constrain( -Angle );
			}

			Invalidate();
		}

		private void onAngleConstraintChanged ( bool isConstrained ) {
			var constraints = ensureValidConstraints();

			if ( constraints is ( Constraint.Angle | Constraint.Velocity ) ) {
				if ( Math.Sign( Velocity * Angle ) == -1 ) Velocity.Constrain( -Velocity );
			}

			Invalidate();
		}

		private void onRadiusConstraintChanged ( bool isConstrained ) {
			ensureValidConstraints();
			Invalidate();
		}

		/// <summary>
		/// Unsigned speed in arclength per ms. This is essentially angle in radians per ms at <see cref="Radius"/> = 1. To set this value you need to set <see cref="Velocity"/>.
		/// </summary>
		public double Speed => Math.Abs( Velocity );

		private double distancePerBeat = Math.Tau;
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

		private Vector2d offset = Vector2d.One / 80;
		/// <summary>
		/// Stacking offset.
		/// </summary>
		[Inspectable( Section = InspectableAttribute.SectionProperties, FormatMethod = nameof( FormatDistance ), ParseMethod = nameof( ParseDistance ) )]
		public Vector2d Offset {
			get => offset;
			set {
				offset = value;
				Invalidate();
			}
		}

		private const double maxAngle = Math.PI * 1.75;
		private const double minAngle = Math.PI * 0;

		protected override void InvalidateProperties () {
			base.InvalidateProperties();

			Radius.Invalidate();
			Angle.Invalidate();
			Velocity.Invalidate();
		}

		[Flags]
		private enum Constraint {
			None = 0,
			Radius = 1,
			Angle = 2,
			Velocity = 4,
			All = Radius | Angle | Velocity
		}

		private static HashSet<Constraint> validConstraints = new() {
			Constraint.None,
			Constraint.Radius,
			Constraint.Angle,
			Constraint.Velocity,
			Constraint.Angle | Constraint.Radius,
			Constraint.Velocity | Constraint.Angle,
			Constraint.Velocity | Constraint.Radius
		};

		private Constraint ensureValidConstraints () {
			Constraint constraints = Constraint.None;
			if ( Radius.IsConstrained )
				constraints |= Constraint.Radius;
			if ( Angle.IsConstrained )
				constraints |= Constraint.Angle;
			if ( Velocity.IsConstrained )
				constraints |= Constraint.Velocity;

			if ( !validConstraints.Contains( constraints ) )
				throw new NotImplementedException( $"Invalid constraint combination: {constraints}" );

			return constraints;
		}

		void recalculate () {
			var constraints = ensureValidConstraints();

			var targetDistance = distancePerBeat * Beats;
			var targetRadius = From.OrbitalState.UnscaledOffsetOfNth( TargetOrbitalIndex ).Length;
			var targetAngle = targetDistance / targetRadius;
			var direction = TargetOrbitalIndex > 0 ? 1 : -1;

			// TODO All of below: This could be calculated with a spiral for a better approximation when changing radius
			// for a simplification, a "good enough" aproximatioin of a spiral might be to sum half-rotation at both start and end radius

			if ( constraints is Constraint.None ) {
				Radius.Value = targetRadius;
				if ( Radius == 0 ) Angle.Value = 0;
				else {
					Angle.Value = Math.Clamp( targetAngle, minAngle, maxAngle ) * direction;
					Radius.Value = Math.Clamp( targetDistance / maxAngle, targetRadius, targetRadius * 2 );
				}

				Velocity.Value = ( Duration == 0 ) ? ( double.PositiveInfinity * direction ) : ( Angle * Radius / Duration );
			}
			else if ( constraints is Constraint.Radius ) {
				targetAngle = Math.Abs( targetDistance / Radius );

				if ( Radius == 0 ) Angle.Value = 0;
				else {
					Angle.Value = Math.Clamp( targetAngle, minAngle, maxAngle ) * direction;
				}

				Velocity.Value = ( Duration == 0 ) ? ( double.PositiveInfinity * direction ) : ( Angle * Math.Abs(Radius) / Duration );
			}
			else if ( constraints is Constraint.Angle ) {
				Radius.Value = Math.Clamp( targetDistance / maxAngle, targetRadius, targetRadius * 2 );

				if ( Math.Sign( targetAngle * Angle ) == -1 ) direction *= -1;
				Velocity.Value = ( Duration == 0 ) ? ( double.PositiveInfinity * direction ) : ( Angle * Radius / Duration );
			}
			else if ( constraints is (Constraint.Angle | Constraint.Radius) ) {
				if ( Math.Sign( targetAngle * Angle ) == -1 ) direction *= -1;
				Velocity.Value = ( Duration == 0 ) ? ( double.PositiveInfinity * direction ) : ( Angle * Math.Abs(Radius) / Duration );
			}
			else if ( constraints is Constraint.Velocity ) {
				targetDistance = Math.Abs( Velocity * Duration );
				targetAngle = targetDistance / targetRadius;

				if ( Math.Sign( direction * Velocity ) == -1 ) direction *= -1;
				Angle.Value = Math.Clamp( targetAngle, minAngle, maxAngle ) * direction;
				Radius.Value = Math.Clamp( targetDistance / maxAngle, targetRadius, targetRadius * 2 );
			}
			else if ( constraints is ( Constraint.Velocity | Constraint.Angle ) ) {
				targetDistance = Math.Abs( Velocity * Duration );

				if ( Angle == 0 ) Radius.Value = targetRadius;
				else Radius.Value = Math.Abs( targetDistance / Angle );
			}
			else if ( constraints is ( Constraint.Velocity | Constraint.Radius ) ) {
				targetDistance = Math.Abs( Velocity * Duration );
				targetAngle = targetDistance / Radius;

				if ( Math.Sign( direction * Velocity ) == -1 ) direction *= -1;
				Angle.Value = targetAngle / Radius * direction;
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
			=> From.OrbitalState.WithScale( From.OrbitalState.Scale + ( targetScale - From.OrbitalState.Scale ) * Math.Clamp( progress, 0, 1 ) ).RotatedBy( Angle * progress ).WithStackingOffset(
				offset * progress
			);

		public override void ApplyDefaults () {
			TargetOrbitalIndex =
				From.FromPreviousIs( x => x is IHasVelocity v && v.Velocity < 0 )
				? -1
				: 1;
		}

		public override ConnectorBlueprint CreateEditorBlueprint ()
			=> new RotationConnectorBlueprint( this );
	}
}
