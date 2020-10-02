using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Patterns.Selectors {
	/// <summary>
	/// Selects multiple ranges with fixed langth that might overlap
	/// </summary>
	public class FixedLengthSelector<T> : RangeSelector<T> {
		int count;
		public Predicate<IEnumerable<T>> Predicate;
		RangeSelector<T> Source;

		public FixedLengthSelector ( int count, Predicate<IEnumerable<T>> predicate, RangeSelector<T> source ) {
			Predicate = predicate;
			Source = source;
			this.count = count;
		}
		public FixedLengthSelector ( int count, Predicate<IEnumerable<T>> predicate ) {
			Predicate = predicate;
			Source = new AllSelector<T>();
			this.count = count;
		}

		public override IEnumerable<IEnumerable<T>> Select ( IEnumerable<T> source ) {
			List<IEnumerable<T>> ranges = new List<IEnumerable<T>>();

			var all = Source.Select( source );

			foreach ( var range in all ) {
				int length = range.Count();
				for ( int i = 0; i < length - count; i++ ) {
					var next = range.Skip( i ).Take( count );
					if ( Predicate( next ) ) ranges.Add( next );
				}
			}

			return ranges;
		}
	}
}
