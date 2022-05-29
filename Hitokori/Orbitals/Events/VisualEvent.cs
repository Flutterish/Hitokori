using osu.Framework.Utils;

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
		public long Order;

		protected VisualEvent ( double startTime, double duration = 0, Easing easing = Easing.None ) {
			StartTime = startTime;
			Duration = duration;
			Easing = easing;
		}

		public bool HasStarted { get; private set; }
		public void Revert () {
			Apply( 0 );
			HasStarted = false;
		}

		public void ApplyAt ( double time ) {
			if ( !HasStarted ) {
				HasStarted = true;
				OnBegin();
			}

			Apply( ProgressAt( time ) );
		}

		protected double ProgressAt ( double time )
			=> Interpolation.ApplyEasing( Easing, Duration == 0 ? 1 : Math.Clamp( ( time - StartTime ) / Duration, 0, 1 ) );

		protected abstract void Apply ( double progress );
		protected virtual void OnBegin () { }

		public abstract IEnumerable<string> Categories { get; }

		public const string CategoryRadius = "Radius";
		public const string CategoryPosition = "Position";
		public const string CategoryAlpha = "Alpha";
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

	public class GenericVisualEvent<Ttarget,T> : VisualEvent where Ttarget : class where T : struct {
		private Func<Ttarget, T> get;
		private Action<Ttarget, T> set;
		private T startValue;
		private T endValue;
		private Ttarget target;
		private string[] categories;

		public GenericVisualEvent ( double startTime, Ttarget target, T endValue, Func<Ttarget, T> get, Action<Ttarget, T> set, string[] categories, double duration = 0, Easing easing = Easing.None ) : base( startTime, duration, easing ) {
			this.endValue = endValue;
			this.target = target;
			this.get = get;
			this.set = set;
			this.categories = categories;
		}

		protected override void Apply ( double progress ) {
			var value = ValueAtProgress( progress );
			set( target, value );
		}
		public T ValueAt ( double time )
			=> ValueAtProgress( ProgressAt( time ) );
		public T ValueAtProgress ( double progress )
			=> Interpolation.ValueAt( progress, startValue, endValue, 0, 1 );

		protected override void OnBegin () {
			startValue = get( target );
		}

		public override IEnumerable<string> Categories => categories;

		public override string ToString ()
			=> $"{( HasStarted ? startValue.ToString() : "???" )} " +
			( double.IsFinite( InterruptedTime )
				? $" -> {ValueAtProgress(ProgressAt(InterruptedTime))} -/> {endValue}"
				: $" -> {endValue}"
			) + $" @ ( {StartTime}" +
			( double.IsFinite( InterruptedTime )
				? $" -> {InterruptedTime} -/> {EndTime}"
				: $" -> {EndTime}"
			) + ")";
	}
}
