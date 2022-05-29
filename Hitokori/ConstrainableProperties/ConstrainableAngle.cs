namespace osu.Game.Rulesets.Hitokori.ConstrainableProperties {
	public class ConstrainableAngle : ConstrainableProperty<double> {
		public bool IsRadians = true;

		public ConstrainableAngle ( Action computeFunction, Action<bool> onConstraintChanged, Action? onValueChanged = null )
			: base( computeFunction, onConstraintChanged, onValueChanged ) { }
		public ConstrainableAngle ( double initialValue, Action computeFunction, Action<bool> onConstraintChanged ) 
			: base( initialValue, computeFunction, onConstraintChanged ) { }

		public double ValueDegrees {
			get => IsRadians ? Value.RadToDeg() : Value;
			set {
				if ( IsRadians ) {
					Value = value.DegToRad();
				}
				else {
					Value = value;
				}
			}
		}

		public double ValueRadians {
			get => IsRadians ? Value : Value.DegToRad();
			set {
				if ( IsRadians ) {
					Value = value;
				}
				else {
					Value = value.RadToDeg();
				}
			}
		}

		public void ConstrainDegrees ( double degrees )
			=> Constrain( IsRadians ? degrees.DegToRad() : degrees );

		public void ConstrainRadians ( double radians )
			=> Constrain( IsRadians ? radians : radians.RadToDeg() );

		public override string StringifyValue () {
			return $"{ValueDegrees:N2}°";
		}
	}
}
