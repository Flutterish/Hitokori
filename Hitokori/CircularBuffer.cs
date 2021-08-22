using System.Collections;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori {
	public class CircularBuffer<T> : IEnumerable<T> {
		private List<T> list;
		private int index;
		public readonly int Length;

		public CircularBuffer ( int length ) {
			list = new List<T>( this.Length = length );
		}

		public void Add ( T item ) {
			if ( Length > list.Count ) {
				list.Add( item );
				return;
			}

			list[ index ] = item;
			index = ( index + 1 ) % Length;
		}

		public IEnumerator<T> GetEnumerator () {
			if ( Length > list.Count ) {
				foreach ( var i in list ) yield return i;
				yield break;
			}

			for ( int i = 0; i < Length; i++ ) {
				yield return list[ ( index + i ) % Length ];
			}
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> ( this as IEnumerable<T> ).GetEnumerator();
	}
}
