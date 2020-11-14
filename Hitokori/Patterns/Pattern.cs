using osu.Game.Rulesets.Hitokori.Patterns.Selectors;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Patterns {
	public abstract class Pattern<TIn, TOut> {
		public abstract RangeSelector<TIn> GetSelector ();
		public abstract IEnumerable<TOut> Apply ( IEnumerable<TIn> selected );

		/// <summary>
		/// Whether no other pattern should generate on top of this one
		/// </summary>
		public virtual bool IsUnique => true;
	}

	public abstract class Pattern<T> : Pattern<T, T> { }
}
