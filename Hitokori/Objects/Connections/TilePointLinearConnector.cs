using osu.Game.Rulesets.Hitokori.ConstrainableProperties;
using osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors;
using osu.Game.Rulesets.Hitokori.Orbitals;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Connections {
	public class TilePointLinearConnector : TilePointConnector, IHasVelocity {
		[Inspectable( Section = InspectableAttribute.SectionProperties, FormatMethod = nameof( FormatDistance ), ParseMethod = nameof( ParseDistance ) )]
		public readonly ConstrainableProperty<double> Distance;
		[Inspectable( Section = InspectableAttribute.SectionProperties, FormatMethod = nameof( FormatVelocity ), ParseMethod = nameof( ParseVelocity ) )]
		public readonly ConstrainableProperty<double> Velocity;
		double IHasVelocity.Velocity => Velocity;

		public TilePointLinearConnector () {
			Distance = new( recalculate, onConstraintChanged );
			Velocity = new( recalculate, onConstraintChanged );

			TargetOrbitalIndex = 0;
		}

		private double angle;
		[Inspectable( Section = InspectableAttribute.SectionProperties, FormatMethod = nameof( FormatAngleRadiansToDegrees ), ParseMethod = nameof( ParseDegreeAngleToRadians ) )]
		public double Angle {
			get => angle;
			set {
				angle = value;
				Invalidate();
			}
		}

		private double distancePerBeat = 120d / 180 * Math.PI;
		[Inspectable( Section = InspectableAttribute.SectionProperties, Label = "Distance per beat", FormatMethod = nameof( FormatDistance ), ParseMethod = nameof( ParseDistance ) )]
		public double DistancePerBeat {
			get => distancePerBeat;
			set {
				distancePerBeat = value;
				Invalidate();
			}
		}

		private void onConstraintChanged ( bool isConstrained )
			=> Invalidate();

		protected override void InvalidateProperties () {
			base.InvalidateProperties();

			if ( Distance.IsConstrained && Velocity.IsConstrained )
				throw new InvalidOperationException( $"Cannot constrian both velocity and distance in a Linear Connector" );

			Distance.Invalidate();
			Velocity.Invalidate();
		}

		private void recalculate () {
			if ( Distance.IsConstrained ) {
				Velocity.Value = Distance / Duration;
			}
			else if ( Velocity.IsConstrained ) {
				Distance.Value = Velocity * Duration;
			}
			else {
				Distance.Value = Math.Min( distancePerBeat * Beats, 5 );
				Velocity.Value = ( Duration == 0 ) ? double.PositiveInfinity : (Distance / Duration);
			}
		}

		public override OrbitalState GetStateAt ( double progress ) => From.OrbitalState.PivotNth(
			0,
			From.OrbitalState.PivotPosition + Angle.AngleToVector( Distance ) * Math.Clamp( progress, 0, 1 )
		);

		public override ConnectorBlueprint CreateEditorBlueprint ()
			=> new LinearConnectorBlueprint( this );
	}
}
