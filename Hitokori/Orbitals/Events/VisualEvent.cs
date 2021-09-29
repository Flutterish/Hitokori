using osu.Framework.Graphics;
using osu.Framework.Utils;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Orbitals.Events {
	/// <summary>
	/// An purely visual event that happened to an orbital
	/// </summary>
	public abstract class VisualEvent {
		public double StartTime;
		public double Duration;
		public double EndTime => StartTime + Duration;

		public Easing Easing;

		public double InterruptedTime = double.PositiveInfinity;
		public VisualEvent? Obscurer;

		protected VisualEvent ( double startTime, double duration = 0, Easing easing = Easing.None ) {
			StartTime = startTime;
			Duration = duration;
			Easing = easing;
		}

		public bool HasStarted { get; private set; }
		public void Revert () {
			HasStarted = false;
		}

		public void ApplyAt ( double time ) {
			if ( !HasStarted ) {
				HasStarted = true;
				OnBegin();
			}

			Apply( Interpolation.ApplyEasing( Easing, Math.Clamp( (time - StartTime) / Duration, 0, 1 ) ) );
		}

		protected abstract void Apply ( double progress );
		protected virtual void OnBegin () { }

		public abstract IEnumerable<string> Categories { get; }

		public const string CategoryRadius = "Radius";
		public const string CategoryPosition = "Position";
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public abstract class VisualEvent<Ttarget> : VisualEvent where Ttarget : class {
		protected Ttarget Target;

		protected VisualEvent ( Ttarget target, double startTime, double duration = 0, Easing easing = Easing.None ) : base( startTime, duration, easing ) {
			Target = target;
		}
	}
}
