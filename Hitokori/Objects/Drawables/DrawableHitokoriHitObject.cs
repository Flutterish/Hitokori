using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public abstract class DrawableHitokoriHitObject : DrawableHitObject<HitokoriHitObject> {
		public DrawableHitokoriHitObject ( HitokoriHitObject hitObject = null ) : base( hitObject ) {
			
		}
	}

	public abstract class DrawableHitokoriHitObject<T> : DrawableHitokoriHitObject where T : HitokoriHitObject {
		new public T HitObject => (T)base.HitObject;
	}
}
