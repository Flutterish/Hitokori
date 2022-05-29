﻿using osu.Framework.Lists;
using System.Collections;

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
	public class Timeline<T,E> : IEnumerable<E>, IReadOnlyList<E> where E : TimelineEntry<T> {
		private SortedList<E> sequence;
		public readonly IComparer<T> Comparer;

		public Timeline ( IComparer<T>? comparer = null ) {
			Comparer = comparer ?? Comparer<T>.Default;

			sequence = new SortedList<E>( (a, b) => Math.Sign( a.StartTime - b.StartTime ) * 2 + Math.Sign( Comparer.Compare( a.Value, b.Value ) ) );
		}

		public virtual int Add ( E entry ) {
			var index = sequence.Add( entry );

			EntryAdded?.Invoke( index, entry );
			return index;
		}

		public int Count => sequence.Count;

		public virtual int Remove ( E entry ) {
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
		public void Clear () {
			while ( sequence.Count > 0 ) {
				RemoveAt( sequence.Count - 1 );
			}
		}

		public int BinarySearch ( E entry )
			=> sequence.BinarySearch( entry );

		public int LastAtOrFirstBefore ( double time ) {
			var i = FirstAfter( time );

			if ( i == -1 ) {
				if ( sequence.Count == 0 || sequence[^1].StartTime > time )
					return -1;
				else
					return sequence.Count - 1;
			}
			else {
				return i - 1;
			}
		}

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

				if ( entry.StartTime >= time ) {
					maxIndex = index;
				}
				else {
					minIndex = index + 1;
				}
			}

			while ( minIndex > 0 && time == sequence[ minIndex - 1 ].StartTime ) {
				minIndex--;
			}

			return minIndex;
		}

		public int FirstAfter ( double time ) {
			var i = FirstAfterOrAt( time );

			if ( i == -1 ) {
				return -1;
			}
			else {
				while ( i < sequence.Count && sequence[ i ].StartTime == time ) {
					i++;
				}

				if ( i >= sequence.Count ) return -1;

				return i;
			}
		}

		/// <summary>
		/// All entries at a given time instant
		/// </summary>
		public IEnumerable<E> EntriesAt ( double time ) {
			var nextIndex = FirstAfterOrAt( time );
			if ( nextIndex != -1 ) {
				while ( nextIndex < Count ) {
					var entry = this[ nextIndex++ ];

					if ( entry.StartTime == time )
						yield return entry;
					else
						break;
				}
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
			=> $"{{{Value}}}@{StartTime}";
	}
}
