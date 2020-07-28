using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {
	public interface IHasTilePosition {
		/// <summary>
		/// Where the tile is. Judgement text appears there by default.
		/// </summary>
		public Vector2 TilePosition { get; }
	}
}
