using osu.Game.Rulesets.Hitokori.Orbitals.Events;

namespace osu.Game.Rulesets.Hitokori.Collections {
	public class AnimatedValue<T> where T : struct {
		private T value;
		private readonly VisualEventSeeker<GenericVisualEvent<AnimatedValue<T>, T>> events = new() { ModifiedBehaviour = TimelineModifiedBehaviour.Replay };

		private static Func<AnimatedValue<T>, T> getFunc = t => t.value;
		private static Action<AnimatedValue<T>, T> setFunc = (t,v) => t.value = v;
		private static string[] categories = new string[] { "Value" };

		public T ValueAt ( double time ) {
			events.CurrentTime = time;
			events.Apply();

			return value;
		}

		public TimelineSeeker<GenericVisualEvent<AnimatedValue<T>, T>>.Entry Animate ( double time, T value, double duration = 0, Easing easing = Easing.None ) {
			return events[ events.Add( time, duration, new GenericVisualEvent<AnimatedValue<T>, T>( time, this, value, getFunc, setFunc, categories, duration, easing ) ) ];
		}
		public void RemoveAnimation ( TimelineSeeker<GenericVisualEvent<AnimatedValue<T>, T>>.Entry entry ) {
			events.Remove( entry );
		}
		public void Clear () {
			events.Clear();
		}

		public IReadOnlyList<TimelineSeeker<GenericVisualEvent<AnimatedValue<T>, T>>.Entry> Animations
			=> events;
	}
}
