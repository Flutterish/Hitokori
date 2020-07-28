

namespace osu.Game.Rulesets.Hitokori.Patterns {
	public class PatternItem<T> {
		public T Value;
		public bool IsProcessed = false;

		public PatternItem ( T value ) {
			Value = value;
		}
	}
}
