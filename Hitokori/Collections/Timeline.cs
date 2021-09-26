using osu.Framework.Lists;
using System;
using System.Collections;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Collections {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class Timeline<T> : Timeline<T, TimelineEntry<T>> {
		public Timeline ( IComparer<T>? comparer = null ) : base( comparer ) { }

		public int Add ( double time, T value ) {
			return Add( new TimelineEntry<T>( time, value ) );
		}
	}

	/// <summary>
	/// Represents a seekable sequence of events spread across time
	/// </summary>
	public class Timeline<T,E> : IEnumerable<E> where E : TimelineEntry<T> {
		private SortedList<E> sequence;
		public readonly IComparer<T> Comparer;

		public Timeline ( IComparer<T>? comparer = null ) {
			Comparer = comparer ?? Comparer<T>.Default;

			sequence = new SortedList<E>( (a, b) => Math.Sign( a.StartTime - b.StartTime ) * 2 + Math.Sign( Comparer.Compare( a.Value, b.Value ) ) );
		}

		public int Add ( E entry ) {
			var index = sequence.Add( entry );

			EntryAdded?.Invoke( index, entry );
			return index;
		}

		public int Count => sequence.Count;

		public int Remove ( E entry ) {
			var index = sequence.BinarySearch( entry );
			sequence.RemoveAt( index );

			EntryRemoved?.Invoke( index, entry );
			return index;
		}
		public E RemoveAt ( int index ) {
			var entry = sequence[ index ];
			Remove( entry );

			return entry;
		}

		public int BinarySearch ( E entry )
			=> sequence.BinarySearch( entry );

		public int FirstBeforeOrAt ( double time ) {
			var i = FirstAfterOrAt( time );

			if ( i == -1 ) {
				return sequence.Count - 1;
			}
			else {
				while ( i >= 0 && sequence[ i ].StartTime > time ) {
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
				while ( i >= 0 && sequence[ i ].StartTime >= time ) {
					i--;
				}
				return i;
			}
		}

		public int FirstAfterOrAt ( double time ) {
			if ( sequence.Count == 0 || sequence[ ^1 ].StartTime < time ) return -1;

			var minIndex = 0;
			var maxIndex = sequence.Count - 1;

			while ( minIndex != maxIndex ) {
				var index = ( maxIndex + minIndex ) / 2;

				var entry = sequence[ index ];

				while ( index > minIndex && entry.StartTime == sequence[ index - 1 ].StartTime ) {
					index--;
					entry = sequence[ index ];
				}

				if ( entry.StartTime >= time ) {
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
				while ( i < sequence.Count && sequence[ i ].StartTime <= time ) {
					i++;
				}

				return i;
			}
		}

		public E this[ int i ] => sequence[ i ];

		public IEnumerator<E> GetEnumerator () {
			return ( (IEnumerable<E>)sequence ).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ( (IEnumerable)sequence ).GetEnumerator();
		}

		public delegate void EntryAddedHandler ( int index, E entry );
		public delegate void EntryRemovedHandler ( int index, E entry );

		public event EntryAddedHandler? EntryAdded;
		public event EntryRemovedHandler? EntryRemoved;
	}

	public class TimelineEntry<T> {
		public readonly double StartTime;
		public readonly T Value;

		public TimelineEntry ( double time, T value ) {
			StartTime = time;
			Value = value;
		}

		public override string ToString ()
			=> $"{StartTime}@{Value}";
	}
}
