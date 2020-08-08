using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {
	public interface IHasDisposeEvent : IDisposable {
		public event Action OnDispose;
	}
}
