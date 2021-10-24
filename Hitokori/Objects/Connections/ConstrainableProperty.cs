using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Connections {
	public class ConstrainableProperty<T> where T : struct {
		public bool IsConstrained { get; private set; }

		private bool isComputed;
		private T value;
		public T Value {
			get {
				if ( !isComputed ) {
					computeAction();
					if ( !isComputed ) 
						throw new InvalidOperationException( "Compute function of a constrainable property did not set its value" );
				}

				return value;
			}
			set {
				isComputed = true;
				this.value = value;
			}
		}

		public void Constrain ( T value ) {
			IsConstrained = true;
			Value = value;
			onConstraintChanged( true );
		}
		public void ReleaseConstraint () {
			IsConstrained = false;
			isComputed = false;
			onConstraintChanged( false );
		}

		public void Invalidate () {
			isComputed = false;
		}

		private Action computeAction;
		private Action<bool> onConstraintChanged;

		public ConstrainableProperty ( Action computeFunction, Action<bool> onConstraintChanged ) {
			isComputed = false;
			IsConstrained = false;
			value = default;
			computeAction = computeFunction;
			this.onConstraintChanged = onConstraintChanged;
		}
		public ConstrainableProperty ( T initialValue, Action computeFunction, Action<bool> onConstraintChanged ) : this( computeFunction, onConstraintChanged ) {
			value = initialValue;
		}

		public static implicit operator T ( ConstrainableProperty<T> p )
			=> p.Value;
	}
}
