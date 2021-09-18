#nullable enable

using osu.Framework.Lists;
using System;
using System.Collections;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Collections {
	/// <summary>
	/// Represents a seekable sequence of events spread across time
	/// </summary>
	public class Timeline<T> : IEnumerable<TimelineEntry<T>> {
		private SortedList<TimelineEntry<T>> sequence;
		public IComparer<T> Comparer = Comparer<T>.Default;

		public Timeline ( IComparer<T>? comparer = null ) {
			Comparer = comparer ?? Comparer;

			sequence = new SortedList<TimelineEntry<T>>( (a, b) => Math.Sign( a.Time - b.Time ) * 2 + Math.Sign( Comparer.Compare( a.Value, b.Value ) ) );
		}

		/// <summary>
		/// Adds an entry and in case of another entry having the same time uses its comparer
		/// </summary>
		public int Add ( double time, T value ) {
			return sequence.Add( new TimelineEntry<T>( time, value ) );
		}

		public int FirstBeforeOrAt ( double time ) {
			var i = FirstAfterOrAt( time );

			if ( i == -1 ) {
				return sequence.Count - 1;
			}
			else {
				while ( i >= 0 && sequence[ i ].Time > time ) {
					i--;
				}
				return i;
			}
		}

		public int FirstBefore ( double time ) {
			var i = FirstAfterOrAt( time );

			if ( i == -1 ) {
				return sequence.Count - 1;
			}
			else {
				while ( i >= 0 && sequence[ i ].Time >= time ) {
					i--;
				}
				return i;
			}
		}

		public int FirstAfterOrAt ( double time ) {
			if ( sequence.Count == 0 || sequence[ ^1 ].Time < time ) return -1;

			var minIndex = 0;
			var maxIndex = sequence.Count - 1;

			while ( minIndex != maxIndex ) {
				var index = ( maxIndex + minIndex ) / 2;

				var entry = sequence[ index ];

				while ( index > minIndex && entry.Time == sequence[ index - 1 ].Time ) {
					index--;
					entry = sequence[ index ];
				}

				if ( entry.Time >= time ) {
					if ( maxIndex == index )
						maxIndex = minIndex;
					else
						maxIndex = index;
				}
				else {
					if ( minIndex == index )
						minIndex = maxIndex;
					else
						minIndex = index;
				}
			}

			return minIndex;
		}

		public int FirstAfter ( double time ) {
			var i = FirstAfterOrAt( time );

			if ( i == -1 ) {
				return -1;
			}
			else {
				while ( i < sequence.Count && sequence[ i ].Time <= time ) {
					i++;
				}

				return i;
			}
		}

		public TimelineEntry<T> this[ int i ] => sequence[ i ];

		public IEnumerator<TimelineEntry<T>> GetEnumerator () {
			return ( (IEnumerable<TimelineEntry<T>>)sequence ).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ( (IEnumerable)sequence ).GetEnumerator();
		}
	}

	public struct TimelineEntry<T> {
		public readonly double Time;
		public readonly T Value;

		public TimelineEntry ( double time, T value ) {
			Time = time;
			Value = value;
		}

		public override string ToString ()
			=> $"{Time}@{Value}";
	}
}
