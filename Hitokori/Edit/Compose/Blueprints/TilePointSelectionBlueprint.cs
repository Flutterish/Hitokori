using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Hitokori.Edit.Compose.SelectionOverlays;
using osu.Game.Rulesets.Hitokori.Objects;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose.Blueprints {
	public class TilePointSelectionBlueprint : HitokoriSelectionBlueprint<TilePoint> {
		private TilePointOverlay overlay;

		public TilePointSelectionBlueprint ( TilePoint item ) : base( item ) {
			InternalChildren = new Drawable[] {
				overlay = new TilePointOverlay( item )
			};
		}

		protected override void OnSelected () {
			base.OnSelected();
			overlay.Show();
		}

		protected override void OnDeselected () {
			base.OnDeselected();
			overlay.Hide();
		}

		public override Quad SelectionQuad => overlay.ScreenSpaceDrawQuad;
	}
}
