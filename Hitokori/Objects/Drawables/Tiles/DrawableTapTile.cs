using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableTapTile : HitokoriTile, IKeyBindingHandler<HitokoriAction> {
		new public readonly TapTile Tile;

		DrawableTilePoint PressPoint;

		public DrawableTapTile ( HitokoriHitObject hitObject ) : base( hitObject ) {
			Tile = hitObject as TapTile;
			this.Center();

			NormalizedTilePosition = Tile.PressPoint.NormalizedTilePosition;
		}

		protected override void UpdateInitialTransforms () { }

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			LifetimeEnd = Tile.PressTime + 1000;
		}

		public bool OnPressed ( HitokoriAction action ) {
			if ( PressPoint.Judged ) return false;
			Hitokori.OnPress();
			PressPoint.TryToHit();
			return true;
		}
		public void OnReleased ( HitokoriAction action ) { }

		protected override void AddNestedHitObject ( DrawableHitObject hitObject ) {
			var tile = hitObject as DrawableTilePoint;

			if ( tile.TilePoint == Tile.PressPoint ) {
				AddInternal( PressPoint = tile );
				PressPoint.Marker.ConnectFrom( Tile.PressPoint.Previous );

				PressPoint.OnNewResult += ( a, b ) => SendClickEvent();
			}
		}

		protected override void ClearNestedHitObjects () {
			PressPoint.Dispose();

			PressPoint = null;
		}
	}
}
