using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {
	public interface IHasDisposeEvent : IDisposable {
		public event Action OnDispose;
	}
}
