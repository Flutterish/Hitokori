using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Patterns.Selectors {
	public abstract class RangeSelector<T> {
		public IEnumerable<IEnumerable<T>> Select ( IEnumerable<IEnumerable<T>> source ) {
			List<IEnumerable<T>> all = new List<IEnumerable<T>>();

			foreach ( var i in source ) {
				all.AddRange( Select( i ) );
			}

			return all;
		}

		public abstract IEnumerable<IEnumerable<T>> Select ( IEnumerable<T> source );
	}
}
