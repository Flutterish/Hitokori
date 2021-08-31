using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.UI.Visuals {
	public class AppliableVisual<T> : CompositeDrawable where T : HitObject {
		private T? appliedHitObject = null;
		public T? AppliedHitObject {
			get => appliedHitObject;
			set {
				if ( value == appliedHitObject ) return;

				if ( appliedHitObject is not null ) {
					free();
					appliedHitObject = null;
				}
				if ( value is not null ) {
					appliedHitObject = value;
					apply( appliedHitObject );
				}
			}
		}
		[MemberNotNullWhen(true, nameof( AppliedHitObject ) )]
		public bool IsApplied => AppliedHitObject is not null;

		void apply ( T hitObject ) {
			Alpha = 1;
			ClearTransforms();
			OnApply( hitObject );
		}

		void free () {
			OnFree();
			ClearTransforms();
			Alpha = 0;
		}

		protected virtual void OnApply ( T hitObject ) { }
		protected virtual void OnFree () { }

		public virtual void UpdateInitialTransforms() { }
	}
}
