using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Hitokori.Edit.SelectionOverlays;
using osu.Game.Rulesets.Hitokori.Objects;

namespace osu.Game.Rulesets.Hitokori.Edit.Blueprints {
	public class TilePointSelectionBlueprint : HitokoriSelectionBlueprint<TilePoint> {
		private TilePointOverlay overlay;

		public TilePointSelectionBlueprint ( TilePoint item ) : base( item ) {
			InternalChildren = new Drawable[] {
				overlay = new TilePointOverlay( item )
			};
		}

		public override Quad SelectionQuad => overlay.ScreenSpaceDrawQuad;
	}
}
