using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Patterns.Selectors {
	public class WhereSelector<T> : RangeSelector<T> {
		Func<T,IEnumerable<T>,bool> Predicate;
		RangeSelector<T> Source;

		public WhereSelector ( Predicate<T> predicate, RangeSelector<T> source ) {
			Predicate = ( a, b ) => predicate( a );
			Source = source;
		}
		public WhereSelector ( Func<T, IEnumerable<T>, bool> predicate, RangeSelector<T> source ) {
			Predicate = predicate;
			Source = source;
		}
		public WhereSelector ( Predicate<T> predicate ) {
			Predicate = ( a, b ) => predicate( a );
			Source = new AllSelector<T>();
		}
		public WhereSelector ( Func<T, IEnumerable<T>, bool> predicate ) {
			Predicate = predicate;
			Source = new AllSelector<T>();
		}

		public override IEnumerable<IEnumerable<T>> Select ( IEnumerable<T> source ) {
			List<IEnumerable<T>> all = new List<IEnumerable<T>>();

			foreach ( var i in Source.Select( source ) ) {
				List<T> group = new List<T>();
				foreach ( var x in i ) {
					if ( Predicate( x, group ) ) {
						group.Add( x );
					} else if ( group.Count > 0 ) {
						all.Add( group );
						group = new List<T>();
					}
				}
				if ( group.Count > 0 ) all.Add( group );
			}

			return all;
		}
	}

	public static class WhereSelectorExtension {
		public static WhereSelector<T> Where<T> ( this RangeSelector<T> source, Predicate<T> predicate )
			=> new WhereSelector<T>( predicate, source );

		public static WhereSelector<T> Where<T> ( this RangeSelector<T> source, Func<T, IEnumerable<T>, bool> predicate )
			=> new WhereSelector<T>( predicate, source );
	}
}
