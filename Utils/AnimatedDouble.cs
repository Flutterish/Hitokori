using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Utils {
	public class AnimatedDouble : Bindable<double> {
		public readonly Drawable Parent;
		public AnimatedDouble ( Drawable parent ) {
			Parent = parent;
		}

		new public double Value {
			get => base.Value;
			set => AnimateTo( value );
		}

		public AnimatedDouble AnimateTo ( double value, double duration = 0, Easing easing = Easing.None ) {
			Parent.TransformBindableTo( this, value, duration, easing );

			return this;
		}

		public double ToDouble ()
			=> base.Value;
		public static implicit operator double ( AnimatedDouble value )
			=> value.ToDouble();
	}

	public class AnimatedVector : Bindable<Vector2> {
		public readonly Drawable Parent;
		private readonly AnimatedDouble x;
		private readonly AnimatedDouble y;

		public AnimatedVector ( Drawable parent ) {
			Parent = parent;

			x = new AnimatedDouble( Parent );
			y = new AnimatedDouble( Parent );

			x.ValueChanged += x => base.Value = new Vector2( (float)x.NewValue, (float)Y );
			y.ValueChanged += y => base.Value = new Vector2( (float)X, (float)y.NewValue );
		}

		new public Vector2 Value {
			get => new Vector2( (float)x, (float)y );
			set => AnimateTo( value );
		}
		public Vector2 Position {
			get => new Vector2( (float)x, (float)y );
			set => AnimateTo( value );
		}

		public double A {
			get => X;
			set => AnimateXTo( value );
		}
		public double B {
			get => Y;
			set => AnimateYTo( value );
		}

		public double X {
			get => x;
			set => AnimateXTo( value );
		}
		public double Y {
			get => y;
			set => AnimateYTo( value );
		}

		public AnimatedVector AnimateXTo ( double value, double duration = 0, Easing easing = Easing.None ) {
			x.AnimateTo( value, duration, easing );

			return this;
		}
		public AnimatedVector AnimateATo ( double value, double duration = 0, Easing easing = Easing.None )
			=> AnimateXTo( value, duration, easing );

		public AnimatedVector AnimateYTo ( double value, double duration = 0, Easing easing = Easing.None ) {
			y.AnimateTo( value, duration, easing );

			return this;
		}
		public AnimatedVector AnimateBTo ( double value, double duration = 0, Easing easing = Easing.None )
			=> AnimateYTo( value, duration, easing );

		public AnimatedVector AnimateTo ( Vector2 value, double duration = 0, Easing easing = Easing.None ) {
			x.AnimateTo( value.X, duration, easing );
			y.AnimateTo( value.Y, duration, easing );

			return this;
		}

		public Vector2 ToVector2 () {
			return Value;
		}
		public static implicit operator Vector2 ( AnimatedVector animated )
			=> animated.ToVector2();
	}
}