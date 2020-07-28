using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

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

		protected override void UpdateStateTransforms ( ArmedState state ) {
			switch ( state ) {
				case ArmedState.Idle:
					break;

				case ArmedState.Miss:
				case ArmedState.Hit:
					LifetimeEnd = Tile.PressTime + 1000;
					break;
			}
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {

		}

		public bool OnPressed ( HitokoriAction action ) {
			if ( Tile.PressPoint.IsNext ) {
				PressPoint.TryToHit();
				return true;
			}
			return false;
		}

		public void OnReleased ( HitokoriAction action ) {
			
		}

		protected override void AddNestedHitObject ( DrawableHitObject hitObject ) {
			var tile = hitObject as DrawableTilePoint;

			if ( tile.TilePoint == Tile.PressPoint ) {
				AddInternal( PressPoint = tile );
				PressPoint.Marker.ConnectFrom( Tile.PressPoint.Previous );

				PressPoint.OnNewResult += ( a, b ) => SendAutoClickEvent();
			}
		}

		protected override void ClearNestedHitObjects () {
			PressPoint.Dispose();
			
			PressPoint = null;
		}
	}
}
