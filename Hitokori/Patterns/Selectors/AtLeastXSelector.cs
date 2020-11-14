using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Patterns.Selectors {
	public class AtLeastXSelector<T> : RangeSelector<T> {
		public int Amout;
		public RangeSelector<T> Source;

		public AtLeastXSelector ( int amout, RangeSelector<T> source ) {
			Amout = amout;
			Source = source;
		}

		public override IEnumerable<IEnumerable<T>> Select ( IEnumerable<T> source )
			=> Source.Select( source ).Where( x => x.Count() >= Amout );
	}

	public static class AtLeastXSelectorExtension {
		public static AtLeastXSelector<T> AtLeast<T> ( this RangeSelector<T> source, int amout )
			=> new AtLeastXSelector<T>( amout, source );
	}
}
