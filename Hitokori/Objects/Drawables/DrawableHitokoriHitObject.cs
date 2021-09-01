using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.UI.Visuals;
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

	public abstract class DrawableHitokoriHitObject<T,TvisualParam,Tvisual> : DrawableHitokoriHitObject 
		where T : TvisualParam
		where TvisualParam : HitokoriHitObject
		where Tvisual : AppliableVisual<TvisualParam>, new() {
		new public T HitObject => (T)base.HitObject;
		protected readonly Tvisual Visual;

		public DrawableHitokoriHitObject () {
			AddInternal( Visual = new() );
		}

		protected override void OnApply () {
			Visual.AppliedHitObject = HitObject;
		}

		protected override void OnFree () {
			Visual.AppliedHitObject = null;
		}

		protected override void UpdateInitialTransforms () {
			Visual.UpdateInitialTransforms();
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			Visual.UpdateHitStateTransforms( state );
			if ( state is ArmedState.Hit or ArmedState.Miss )
				LifetimeEnd = Visual.LatestTransformEndTime;
		}
	}

	public abstract class DrawableHitokoriHitObject<T, Tvisual> : DrawableHitokoriHitObject<T, T, Tvisual>
		where T : HitokoriHitObject
		where Tvisual : AppliableVisual<T>, new() {
	
	}
}
