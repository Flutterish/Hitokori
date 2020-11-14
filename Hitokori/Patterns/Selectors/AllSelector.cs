using osu.Framework.Extensions.IEnumerableExtensions;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Patterns.Selectors {
	public class AllSelector<T> : RangeSelector<T> {
		public override IEnumerable<IEnumerable<T>> Select ( IEnumerable<T> source )
			=> source.Yield();
	}
}
