using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class Trail : Container { // TODO trails should interpolate angles so they dont jump an a late/early input
		public Path Line;
		public Vector2 Offset;
		LoopingList<Vector2> vertices = new LoopingList<Vector2>( 100 ); // TODO make trails shorten towards the end
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
		}

		public void AddVertice ( Vector2 position ) {
			vertices.Add( position );
		}

		protected override void Update () {
			Line.ClearVertices();

			foreach ( var i in vertices ) {
				Line.AddVertex( i - Offset );
			}

			Line.Position = -Line.PositionInBoundingBox( Vector2.Zero );
		}
	}
}
