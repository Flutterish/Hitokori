using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public abstract class DrawableHitokoriHitObject : DrawableHitObject<HitokoriHitObject> {
		public DrawableHitokoriHitObject ( HitokoriHitObject hitObject = null ) : base( hitObject ) {
			AutoSizeAxes = Axes.Both;
		}
	}

	public abstract class DrawableHitokoriHitObject<T> : DrawableHitokoriHitObject where T : HitokoriHitObject {
		new public T HitObject => (T)base.HitObject;
	}
}
