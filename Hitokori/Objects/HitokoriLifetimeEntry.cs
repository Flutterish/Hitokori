using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Hitokori.Objects {
	public class HitokoriLifetimeEntry : HitObjectLifetimeEntry {
		public HitokoriLifetimeEntry ( HitObject hitObject ) : base( hitObject ) {
		}

		protected override double InitialLifetimeOffset => HitObject is TilePoint tp ? tp.LifetimeOffset : 2000;
	}
}
