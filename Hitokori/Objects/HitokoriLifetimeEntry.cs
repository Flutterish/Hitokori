using osu.Game.Rulesets.Objects;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects {
	public class HitokoriLifetimeEntry : HitObjectLifetimeEntry {
		public HitokoriLifetimeEntry ( HitObject hitObject ) : base( hitObject ) {
		}

		protected override double InitialLifetimeOffset => Math.Max( 2000, HitObject is TilePoint tp ? ( tp.FromPrevious is null ? 0 : tp.FromPrevious.Duration ) : 0 );
	}
}
