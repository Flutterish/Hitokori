using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {

	public abstract class DrawableHitokoriHitObject : DrawableHitObject<HitokoriHitObject>, IHasDisposeEvent {
		protected DrawableHitokoriHitObject ( HitokoriHitObject hitObject = null ) : base( hitObject ) { }

		[Resolved]
		public DrawableHitokori Hitokori { get; private set; }
		[Resolved]
		public HitokoriPlayfield Playfield { get; private set; }
		new public readonly Bindable<Colour4> AccentColour = new();

		[BackgroundDependencyLoader]
		private void load ( Bindable<Colour4> accent ) {
			AccentColour.BindTo( accent );
		}

		protected override double InitialLifetimeOffset => 1000;

		public event Action OnDispose;
		protected override void Dispose ( bool isDisposing ) {
			base.Dispose( isDisposing );
			OnDispose?.Invoke();
			OnDispose = null;
		}

		protected override DrawableHitObject CreateNestedHitObject ( HitObject hitObject )
			=> ( hitObject as HitokoriHitObject ).AsDrawable();
	}
}
