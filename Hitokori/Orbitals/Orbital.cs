using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.UI;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Orbitals {
	public class Orbital : CompositeDrawable {
		[Resolved]
		private HitokoriPlayfield playfield { get; set; }

		private Circle circle;
		private Trail trail;
		
		public Orbital () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AddInternal( trail = new Trail() );
			AddInternal( circle = new Circle {
				Size = new Vector2( 20 ),
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre
			} );
		}

		private double verticeInterval = 8;
		private double accumulatedTime;

		protected override bool ComputeIsMaskedAway ( RectangleF maskingBounds )
			=> false;

		protected override void Update () {
			base.Update();

			accumulatedTime += Time.Elapsed;
			trail.Offset = Position + Parent.Position;
			while ( accumulatedTime >= verticeInterval ) {
				accumulatedTime -= verticeInterval;

				trail.AddVertice( trail.Offset );
			}
		}
	}
}
