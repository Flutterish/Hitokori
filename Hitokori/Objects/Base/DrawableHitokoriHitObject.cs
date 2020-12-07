using osu.Framework.Allocation;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {

	public abstract class DrawableHitokoriHitObject : DrawableHitObject<HitokoriHitObject>, IHasDisposeEvent {
		protected DrawableHitokoriHitObject ( HitokoriHitObject hitObject ) : base( hitObject ) { }

		[Resolved]
		public HitokoriPlayfield Playfield { get; private set; }

		protected bool isCurrent ( TilePoint tilePoint )
			=> Playfield.NextTilePoint == tilePoint?.Next;
		protected bool isNext ( TilePoint tilePoint )
			=> Playfield.NextTilePoint == tilePoint;
		protected bool isPrevious ( TilePoint tilePoint )
			=> Playfield.NextTilePoint?.Previous == tilePoint?.Next;

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
	}
}
