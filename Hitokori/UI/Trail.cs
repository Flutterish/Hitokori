using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Primitives;
using osuTK;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class Trail : CompositeDrawable {
		public Path Line;
		public Vector2 Offset;
		CircularBuffer<Vector2> vertices = new CircularBuffer<Vector2>( 100 );
		public int VerticeCount => vertices.Length;

		public Trail () {
			InternalChildren = new Drawable[] {
				Line = new Path {
					PathRadius = 6,
					Alpha = 0.3f
				}
			};

			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;
			AutoSizeAxes = Axes.None;

			Line.Anchor = Anchor.TopLeft;
			Line.Origin = Anchor.TopLeft;
			Line.AutoSizeAxes = Axes.None;
			Line.Size = new Vector2( 400 );
		}

		protected override bool ComputeIsMaskedAway ( RectangleF maskingBounds )
			=> false;

		public void AddVertice ( Vector2 position ) {
			vertices.Add( position );
		}

		protected override void Update () {
			Line.ClearVertices();

			foreach ( var i in vertices ) {
				Line.AddVertex( i - Offset );
			}

			var requiredSize = getRequiredSize();

			if ( requiredSize.X > Line.Size.X || requiredSize.Y > Line.Size.Y ) {
				Line.Size = new Vector2( MathF.Max( requiredSize.X, Line.Size.X ), MathF.Max( requiredSize.Y, Line.Size.Y ) );
				//Size = requiredSize;
			}

			Line.Position = -Line.PositionInBoundingBox( Vector2.Zero );
		}

		private Vector2 getRequiredSize () {
			var points = Line.Vertices;
			if ( !points.Any() ) return Vector2.One;
			return new Vector2(
				points.Max( v => v.X ) - points.Min( v => v.X ),
				points.Max( v => v.Y ) - points.Min( v => v.Y )
			) + new Vector2( Line.PathRadius * 2 );
		}
	}
}
