using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Hitokori.Collections;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.UI.Paths {
	public class Trail : CompositeDrawable {
		private FixedSizePath line;
		public Vector2 Offset;
		public float PathRadius = 6;
		CircularBuffer<Vector2> vertices = new CircularBuffer<Vector2>( 100 );
		public int VerticeCount => vertices.Capacity;

		public Trail () {
			InternalChildren = new Drawable[] {
				line = new FixedSizePath() {
					BufferSize = new Vector2( 500 )
				}
			};

			Anchor = Anchor.Centre;
			Origin = Anchor.TopLeft;
			AutoSizeAxes = Axes.Both;

			line.Anchor = Anchor.TopLeft;
			line.Origin = Anchor.TopLeft;
			line.AutoSizeAxes = Axes.Both;
			Alpha = 0.3f;
		}

		protected override bool ComputeIsMaskedAway ( RectangleF maskingBounds )
			=> false;

		bool isInvalidated = false;
		public void AddVertice ( Vector2 position ) {
			isInvalidated = true;
			vertices.Add( position );
		}

		public void Rescale ( float scale ) {
			isInvalidated = true;
			for ( int i = 0; i < vertices.Count; i++ ) {
				vertices[ i ] *= scale;
			}
		}

		protected override void Update () {
			if ( isInvalidated )
				FlushVertices();
		}

		public void FlushVertices () {
			isInvalidated = false;
			line.ClearVertices();

			line.PathRadius = PathRadius;
			foreach ( var i in vertices ) {
				line.AddVertex( i - Offset );
			}

			Position = -line.PositionInBoundingBox( Vector2.Zero );
		}
	}
}
