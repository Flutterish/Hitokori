using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class HitokoriHitObjectLifetimeEntry : HitObjectLifetimeEntry {
		public HitokoriHitObjectLifetimeEntry ( HitObject hitObject ) : base( hitObject ) { }

		protected override double InitialLifetimeOffset => 1000;
	}
}
