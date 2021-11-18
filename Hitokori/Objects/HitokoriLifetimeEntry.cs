using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Hitokori.Objects {
	public class HitokoriLifetimeEntry : HitObjectLifetimeEntry {
		public readonly double MinumumLifetimeOffset;

		public HitokoriLifetimeEntry ( HitObject hitObject, double initialLifetimeOffset = 2000 ) : base( hitObject ) {
			MinumumLifetimeOffset = initialLifetimeOffset;
			if ( HitObject is TilePoint tp )
				tp.MinumumLifetimeOffset = MinumumLifetimeOffset;

			SetLifetimeStart( HitObject.StartTime - InitialLifetimeOffset );
		}

		protected override double InitialLifetimeOffset => HitObject is TilePoint tp ? tp.LifetimeOffset : MinumumLifetimeOffset;
	}
}
