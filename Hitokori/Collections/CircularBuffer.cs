using System.Collections;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Collections
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private List<T> list;
        private int index;
        public readonly int Capacity;
        public int Count => list.Count;

        public CircularBuffer(int length)
        {
            list = new List<T>(Capacity = length);
        }

        public void Add(T item)
        {
            if (Capacity > list.Count)
            {
                list.Add(item);
                return;
            }

            list[index] = item;
            index = (index + 1) % Capacity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (Capacity > list.Count)
            {
                foreach (var i in list) yield return i;
                yield break;
            }

            for (int i = 0; i < Capacity; i++)
            {
                yield return list[(index + i) % Capacity];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => (this as IEnumerable<T>).GetEnumerator();

        public T this[int i]
        {
            get => list[(index + i).Mod(Count)];
            set => list[(index + i).Mod(Count)] = value;
        }
    }
}
