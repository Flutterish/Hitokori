using osu.Game.Rulesets.Hitokori.Patterns.Selectors;
using osu.Game.Rulesets.Hitokori.Utils;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Patterns {
	public class PatternGeneratorResult<TIn, TOut> {
		public List<TIn> Unprocessed = new List<TIn>();
		public List<TOut> Processed = new List<TOut>();
	}
	public class PatternGeneratorResult<T> : PatternGeneratorResult<T, T> { }

	public class PatternGenerator<T> where T : class {
		List<Pattern<T>> Patterns = new List<Pattern<T>>();

		public delegate void BatchEvent ( IEnumerable<T> original, IEnumerable<T> processed );
		public event BatchEvent OnBatch;

		public void AddPattern ( Pattern<T> pattern ) {
			Patterns.Add( pattern );
		}

		public void ClearPatterns () {
			Patterns.Clear();
		}

		public PatternGeneratorResult<T> Process ( IEnumerable<T> source ) {
			List<T> elements = source.ToList();

			var hash = new Dictionary<T, PatternItem<T>>();
			foreach ( var i in elements ) {
				hash.Add( i, new PatternItem<T>( i ) );
			}

			foreach ( var pattern in Patterns ) {
				var avaiable = pattern.GetSelector().Select( new WhereSelector<T>( x => !hash[ x ].IsProcessed ).Select( elements ) );

				foreach ( var range in avaiable ) {
					var processed = pattern.Apply( range );

					hash.AddNewKeys( processed, x => new PatternItem<T>( x ) );
					if ( pattern.IsUnique ) {
						foreach ( var i in processed ) {
							hash[ i ].IsProcessed = true;
						}
					}

					OnBatch?.Invoke( range, processed );

					int index = elements.IndexOf( range.First() );
					elements.RemoveRange( index, range.Count() );
					elements.InsertRange( index, processed ); // TODO probably replace with linked list
				}
			}

			var result = new PatternGeneratorResult<T>();
			result.Processed = elements.Except( source ).ToList();
			result.Unprocessed = source.Except( elements ).ToList();
			return result;
		}
	}
}
