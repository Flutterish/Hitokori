using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {
	public abstract class HitokoriTile : DrawableHitokoriHitObject, IHasTilePosition {
		public HitokoriTileObject Tile => HitObject as HitokoriTileObject;

		protected HitokoriTile ( HitokoriHitObject hitObject ) : base( hitObject ) {
			OnNewResult += ( maybeTile, _ ) => {
				if ( maybeTile is DrawableTilePoint tile && tile.TilePoint == Tile.LastPoint )
					ApplyResult( j => j.Type = HitResult.IgnoreHit );
			};
		}

		public virtual void ChildTargeted ( DrawableTilePoint child ) { }
		public virtual void ChildUntargeted ( DrawableTilePoint child ) { }

		public const float SPACING = 140;
		public const float SIZE = 15;

		public virtual Vector2 NormalizedTilePosition { get; protected set; }
		public Vector2 TilePosition {
			get => NormalizedTilePosition * SPACING;
			protected set => NormalizedTilePosition = value / SPACING;
		}
	}
}
