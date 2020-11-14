using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {

	public abstract class DrawableHitokoriHitObject : DrawableHitObject<HitokoriHitObject>, IHasDisposeEvent {
		protected DrawableHitokoriHitObject ( HitokoriHitObject hitObject ) : base( hitObject ) { }

		protected override double InitialLifetimeOffset => 1000;

		public event Action OnDispose;
		protected override void Dispose ( bool isDisposing ) {
			base.Dispose( isDisposing );
			OnDispose?.Invoke();
		}

		protected override DrawableHitObject CreateNestedHitObject ( HitObject hitObject )
			=> ( hitObject as HitokoriHitObject ).AsDrawable();

		public void RemoveNested () {
			ClearNestedHitObjects();
		}

		protected override void LoadComplete () {
			base.LoadComplete();
			LifetimeStart = HitObject.StartTime - InitialLifetimeOffset;
		}
	}
}
