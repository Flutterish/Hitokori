using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableTapTile : HitokoriTile, IKeyBindingHandler<HitokoriAction> {
		new public TapTile Tile => HitObject as TapTile;

		DrawableTilePoint PressPoint;

		public DrawableTapTile ( HitokoriHitObject hitObject ) : base( hitObject ) {
			this.Center();
		}
		public DrawableTapTile () : this( null ) { }
		protected override void OnApply () {
			base.OnApply();
			NormalizedTilePosition = Tile.PressPoint.NormalizedTilePosition;

			PressPoint.Marker.ConnectFrom( Tile.PressPoint.Previous );
			PressPoint.OnNewResult += ( a, b ) => SendClickEvent();
		}
		protected override void UpdateInitialTransforms () { }

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			LifetimeEnd = Tile.PressTime + 1000;
		}
		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( userTriggered ) {
				Hitokori.OnPress();
				PressPoint.TryToHitAtOffset( timeOffset );
			}
		}
		public bool OnPressed ( HitokoriAction action ) {
			if ( PressPoint.Judged ) return false;
			UpdateResult( userTriggered: true );
			return true;
		}
		public void OnReleased ( HitokoriAction action ) { }

		protected override void AddNestedHitObject ( DrawableHitObject hitObject ) {
			AddInternal( PressPoint = hitObject as DrawableTilePoint );
		}

		protected override void ClearNestedHitObjects () {
			RemoveInternal( PressPoint );

			PressPoint = null;
		}
	}
}
