using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails {
    public abstract class Connector : CompositeDrawable
    {
		public Connector () {
			progress = new( this );
			progress.BindValueChanged( v => isInvalidated = true );
		}

		protected bool isInvalidated = true;
		protected AnimatedVector progress;
		private float trailRadius = HitokoriTile.SIZE / 8f;
		public float TrailRadius {
			get => trailRadius;
			set {
				if ( trailRadius == value ) return;

				trailRadius = value;
				isInvalidated = true;
			}
		}

		protected abstract void render ();

		protected override void Update () {
			if ( isInvalidated ) {
				isInvalidated = false;
				render();
			}

			base.Update();
		}

		public void Connect ( double duration, Easing easing = Easing.None ) {
			progress.AnimateBTo( 1, duration, easing );
		}
		public void Disconnect ( double duration, Easing easing = Easing.None ) {
			progress.AnimateATo( 1, duration, easing );
		}
		public void Reset () {
			ClearTransforms( true );
			progress.A = 0;
			progress.B = 0;
		}

		public virtual double Appear ( double duration = 500, Easing easing = Easing.In ) {
			this.FadeInFromZero( duration );
			Connect( duration, easing );

			return duration;
		}
		public virtual double Disappear ( double duration = 300, Easing easing = Easing.Out ) {
			this.FadeOut( duration );
			Disconnect( duration, easing );

			return duration;
		}
	}
}
