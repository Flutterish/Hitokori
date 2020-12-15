using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails {
	public class Trail : CompositeDrawable {
		public Path Line;
		public Vector2 Offset;
		LoopingList<Vector2> vertices = new LoopingList<Vector2>( 100 ); // TODO make trails less opaque towards the end
		public int VerticeCount => vertices.Length;

		new public Colour4 Colour {
			get => Line.Colour;
			set => Line.Colour = value;
		}
		public Trail () {
			InternalChildren = new Drawable[] {
				Line = new Path {
					PathRadius = HitokoriTile.SIZE / 3f,
					Alpha = 0.3f
				}
			};

			this.Center();
			Line.Anchor = Anchor.TopLeft;
			Line.Origin = Anchor.TopLeft;
			Line.AutoSizeAxes = Axes.None;
			AutoSizeAxes = Axes.None;
			Line.Size = new Vector2( 400 );
		}

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
