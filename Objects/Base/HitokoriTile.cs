using osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {
	public abstract class HitokoriTile : DrawableHitokoriHitObject, IHasTilePosition {
		public DrawableHitokori Hitokori;
		public readonly HitokoriTileObject Tile;

		protected HitokoriTile ( HitokoriHitObject hitObject ) : base( hitObject ) {
			Tile = hitObject as HitokoriTileObject;

			OnNewResult += OnResult;
		}

		private void OnResult ( Rulesets.Objects.Drawables.DrawableHitObject arg1, JudgementResult arg2 ) {
			if ( arg1 is DrawableTilePoint tile && tile.TilePoint == Tile.LastPoint ) {
				ApplyResult( j => {
					j.Type = arg2.Type;
				} );
			}
		}

		#region Constants
		public const float SPACING = 140;
		public const float SIZE = 15;
		public static Vector2 RIGHT => new Vector2( SPACING, 0 );
		public static Vector2 LEFT => new Vector2( -SPACING, 0 );
		public static Vector2 UP => new Vector2( 0, -SPACING );
		public static Vector2 DOWN => new Vector2( 0, SPACING );
		#endregion
		#region Positioning
		public Vector2 NormalizedTilePosition;
		public Vector2 TilePosition {
			get => NormalizedTilePosition * SPACING;
			set => NormalizedTilePosition = value / SPACING;
		}
		#endregion

		public delegate void OnClickEvent ( HitokoriTile tile, AutoClickType type );
		public event OnClickEvent OnAutoClick;
		protected void SendAutoClickEvent ( AutoClickType type = AutoClickType.Press ) {
			OnAutoClick?.Invoke( this, type );
		}

		public void TryToSetResult ( DrawableTilePoint tile, HitResult result ) {
			if ( result != HitResult.None ) { // Just completely dismiss the "cant be none" check lmao
				tile.SetResult( result );
			}
		}

		public void SetResultOrMiss ( DrawableTilePoint tile, HitResult result ) {
			if ( result != HitResult.None ) {
				tile.SetResult( result );
			} else {
				tile.SetResult( HitResult.Miss );
			}
		}
	}
}
