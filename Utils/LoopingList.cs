using System.Collections;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Utils {
	public class LoopingList<T> : IEnumerable<T> {
		private List<T> list;
		private int index;
		private int length;

		public LoopingList ( int length ) {
			list = new List<T>( this.length = length );
		}

		public void Add ( T item ) {
			if ( length > list.Count ) {
				list.Add( item );
				return;
			}

			list[ index ] = item;
			index = ( index + 1 ) % length;
		}

		public IEnumerator<T> GetEnumerator () {
			if ( length > list.Count ) {
				foreach ( var i in list ) yield return i;
				yield break;
			}

			for ( int i = 0; i < length; i++ ) {
				yield return list[ ( index + i ) % length ];
			}
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> ( this as IEnumerable<T> ).GetEnumerator();
	}
}
