using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableTapTile : HitokoriTile, IKeyBindingHandler<HitokoriAction> {
		new public TapTile Tile => HitObject as TapTile;
		public override Vector2 NormalizedTilePosition => Tile.PressPoint.NormalizedTilePosition;

		DrawableTilePoint PressPoint;

		public DrawableTapTile () : base( null ) {
			this.Center();
		}
		protected override void OnApply () {
			base.OnApply();

			PressPoint.Marker.ConnectFrom( Tile.PressPoint.Previous );
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			LifetimeEnd = Tile.PressTime + 1000;
		}
		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( userTriggered ) {
				Hitokori.OnPress();
				PressPoint.TryToHitAtOffset( timeOffset );
			}
		}
		public bool OnPressed ( KeyBindingPressEvent<HitokoriAction> action ) {
			if ( PressPoint.Judged ) return false;
			Playfield.Click( AutoClickType.Press );
			UpdateResult( userTriggered: true );
			return true;
		}
		public void OnReleased ( KeyBindingReleaseEvent<HitokoriAction> action ) { }

		protected override void AddNestedHitObject ( DrawableHitObject hitObject ) {
			AddInternal( PressPoint = hitObject as DrawableTilePoint );
		}

		protected override void ClearNestedHitObjects () {
			RemoveInternal( PressPoint );

			PressPoint = null;
		}
	}
}
