using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DisposableContainer : Container, IHasDisposeEvent {
		public event Action OnDispose;

		protected override void Dispose ( bool isDisposing ) {
			base.Dispose( isDisposing );
			OnDispose?.Invoke();
		}
	}
}
