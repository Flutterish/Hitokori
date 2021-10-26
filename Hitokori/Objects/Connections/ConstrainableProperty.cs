using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Connections {
	public class ConstrainableProperty<T> where T : struct {
		public bool IsConstrained { get; private set; }

		public bool IsComputed { get; private set; }
		private T value;
		public T Value {
			get {
				if ( !IsComputed ) {
					computeAction();
					if ( !IsComputed ) 
						throw new InvalidOperationException( "Compute function of a constrainable property did not set its value" );
				}

				return value;
			}
			set {
				if ( IsConstrained ) 
					throw new InvalidOperationException( "Cannot set a value of a constrained property" );

				IsComputed = true;
				this.value = value;
				onValueChanged?.Invoke();
			}
		}

		public void Constrain ( T value ) {
			Value = value;
			IsConstrained = true;
			onConstraintChanged( true );
		}
		public void ReleaseConstraint () {
			IsConstrained = false;
			IsComputed = false;
			onConstraintChanged( false );
		}

		public void Invalidate () {
			IsComputed = IsConstrained;
		}

		private Action computeAction;
		private Action? onValueChanged;
		private Action<bool> onConstraintChanged;

		public ConstrainableProperty ( Action computeFunction, Action<bool> onConstraintChanged, Action? onValueChanged = null ) {
			IsComputed = false;
			IsConstrained = false;
			value = default;
			computeAction = computeFunction;
			this.onConstraintChanged = onConstraintChanged;

			this.onValueChanged = onValueChanged;
		}
		public ConstrainableProperty ( T initialValue, Action computeFunction, Action<bool> onConstraintChanged ) : this( computeFunction, onConstraintChanged ) {
			value = initialValue;
		}

		public static implicit operator T ( ConstrainableProperty<T> p )
			=> p.Value;
	}
}
