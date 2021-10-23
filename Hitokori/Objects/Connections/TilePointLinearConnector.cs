using osu.Game.Rulesets.Hitokori.Orbitals;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Connections {
	public class TilePointLinearConnector : TilePointConnector, IHasVelocity {
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

		public override OrbitalState GetStateAt ( double progress ) => From.OrbitalState.PivotNth(
			0,
			From.OrbitalState.PivotPosition + Angle.AngleToVector( DistancePerBeat * Beats ) * progress
		);

		public double Velocity => DistancePerBeat * Beats / Duration;
	}
}
