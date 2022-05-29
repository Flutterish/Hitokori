namespace osu.Game.Rulesets.Hitokori {
	public class BindablePool<T> {
		private Stack<Bindable<T>> pool = new();
		public Bindable<T> Rent () {
			if ( pool.TryPop( out var b ) )
				return b;
			else
				return new();
		}
		public void Return ( Bindable<T> bindable ) {
			bindable.UnbindAll();
			pool.Push( bindable );
		}
	}
}
