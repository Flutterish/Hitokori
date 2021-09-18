using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Orbitals.Events {
	/// <summary>
	/// An purely visual event that happened to an orbital
	/// </summary>
	public abstract class VisualEvent {
		public VisualEvent ( double time ) {
			Time = time;
		}

		public readonly double Time;
		public abstract IEnumerable<string> Categories { get; }
	}
}
