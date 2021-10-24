using osu.Game.Rulesets.Hitokori.Orbitals;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Connections {
	public class TilePointLinearConnector : TilePointConnector, IHasVelocity {
		public readonly ConstrainableProperty<double> Distance;
		public readonly ConstrainableProperty<double> Velocity;
		double IHasVelocity.Velocity => Velocity;

		public TilePointLinearConnector () {
			Distance = new ConstrainableProperty<double>( recalculate, onConstraintChanged );
			Velocity = new ConstrainableProperty<double>( recalculate, onConstraintChanged );
		}

		private double angle;
		public double Angle {
			get => angle;
			set {
				angle = value;
				Invalidate();
			}
		}

		private double distancePerBeat = 120d / 180 * Math.PI;
		public double DistancePerBeat {
			get => distancePerBeat;
			set {
				distancePerBeat = value;
				Invalidate();
			}
		}

		private void onConstraintChanged ( bool isConstrained )
			=> Invalidate();

		private void recalculate () {
			if ( Distance.IsConstrained && Velocity.IsConstrained )
				throw new InvalidOperationException( $"Cannot constrian both velocity and distance in a {nameof(TilePointLinearConnector)}" );
			else if ( Distance.IsConstrained ) {
				Velocity.Value = Distance / Duration;
			}
			else if ( Velocity.IsConstrained ) {
				Distance.Value = Velocity * Duration;
			}
			else {
				Distance.Value = Math.Min( distancePerBeat * Beats, 5 );
				Velocity.Value = Distance / Duration;
			}
		}

		public override OrbitalState GetStateAt ( double progress ) => From.OrbitalState.PivotNth(
			0,
			From.OrbitalState.PivotPosition + Angle.AngleToVector( Distance ) * Math.Clamp( progress, 0, 1 )
		);
	}
}
