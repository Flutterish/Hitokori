using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Connections {
	public interface IHasVelocity {
		double Velocity { get; }
		public double Speed => Math.Abs( Velocity );
	}
}
