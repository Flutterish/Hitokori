using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Hitokori.Collections;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class Trail : CompositeDrawable {
		public Path Line;
		public Vector2 Offset;
		new public Vector2 Position;
		CircularBuffer<Vector2> vertices = new CircularBuffer<Vector2>( 100 );
		public int VerticeCount => vertices.Capacity;

		public Trail () {
			InternalChildren = new Drawable[] {
				Line = new Path {
					PathRadius = 6,
					Alpha = 0.3f
				}
			};

			Anchor = Anchor.Centre;
			Origin = Anchor.TopLeft;
			AutoSizeAxes = Axes.None;

			Line.Anchor = Anchor.TopLeft;
			Line.Origin = Anchor.TopLeft;
			Line.AutoSizeAxes = Axes.None;
			Line.Size = new Vector2( 400 );
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
			Line.ClearVertices();

			foreach ( var i in vertices ) {
				Line.AddVertex( i - Offset );
			}

			var requiredSize = Size = getRequiredSize();

			if ( requiredSize.X > Line.Size.X || requiredSize.Y > Line.Size.Y ) {
				Line.Size = new Vector2( MathF.Max( requiredSize.X, Line.Size.X ), MathF.Max( requiredSize.Y, Line.Size.Y ) );
			}

			base.Position = Position - Line.PositionInBoundingBox( Vector2.Zero );
		}

		private Vector2 getRequiredSize () {
			var points = Line.Vertices;
			if ( points.Count == 0 ) return Vector2.One;
			float maxX = float.NegativeInfinity;
			float maxY = float.NegativeInfinity;
			float minX = float.PositiveInfinity;
			float minY = float.PositiveInfinity;

			for ( int i = 0; i < points.Count; i++ ) {
				var p = points[ i ];
				if ( maxX < p.X ) maxX = p.X;
				if ( minX > p.X ) minX = p.X;
				if ( maxY < p.Y ) maxY = p.Y;
				if ( minY > p.Y ) minY = p.Y;
			}

			return new Vector2(
				maxX - minX + Line.PathRadius * 2,
				maxY - minY + Line.PathRadius * 2
			);
		}
	}
}
