using osu.Framework.Extensions.TypeExtensions;
using System;

namespace osu.Game.Rulesets.Hitokori.ConstrainableProperties {
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
			IsConstrained = true;
			this.value = value;
			IsComputed = true;
			onConstraintChanged( true );
			onValueChanged?.Invoke();
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

		public override string ToString ()
			=> $"Constrainable {typeof(T).ReadableName()} {{ {(IsComputed ? $"Value = {StringifyValue()}" : "Not computed")} | {(IsConstrained ? "Constrained" : "Not constrained")} }}";

		public virtual string StringifyValue ()
			=> Newtonsoft.Json.JsonConvert.SerializeObject( Value );

		public virtual T ParseValue ( string text )
			=> Newtonsoft.Json.JsonConvert.DeserializeObject<T>( text );
	}
}
