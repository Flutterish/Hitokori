using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using osuTK.Audio.OpenAL;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	/// <summary>
	/// A connector is a path between 2 tiles. Its start position is offset by whatever the offset is from the "from" tile, that is
	/// if you set its position to `<c>from.HoldPosition - parent.Position</c>` its start will be centered at "from";
	/// </summary>
	public class Connector : DisposableContainer {
		public TilePoint From;
		public TilePoint To;
		public Path Line;

		new public double Alpha;

		protected AnimatedVector Progress;

		new public Colour4 Colour {
			get => Line.Colour;
			set => Line.Colour = value;
		}

		public Connector ( TilePoint from, TilePoint to, float alpha = 0.2f ) {
			Progress = new AnimatedVector( parent: this );

			InternalChildren = new Drawable[] {
				Line = new Path {
					PathRadius = HitokoriTile.SIZE / 8f,
					Alpha = alpha,
					Depth = 100
				}
			};

			Alpha = alpha;

			this.Center();
			Line.Anchor = Anchor.TopLeft;
			Line.Origin = Anchor.TopLeft;

			From = from;
			To = to;

			Progress.ValueChanged += x => {
				if ( Line != null ) {
					UpdateConnector();
				}
			};
		}

		protected virtual void UpdateConnector () {
			Line.ClearVertices();

			Line.AddVertex( ( To.TilePosition - From.TilePosition ) * (float)Progress.A );
			Line.AddVertex( ( To.TilePosition - From.TilePosition ) * (float)Progress.B );

			Line.Position = -Line.PositionInBoundingBox( Vector2.Zero );
		}

		public void Connect ( double duration, Easing easing = Easing.None ) {
			Progress.AnimateBTo( 1, duration, easing );
		}

		public virtual double Appear () {
			this.FadeInFromZero( 500 );
			Connect( 500, Easing.In );

			return 500;
		}

		public virtual double Disappear () {
			this.FadeOut( 300 ).OnComplete( x => {
				if ( Line != null ) {
					Remove( Line );
					Line.Dispose();
					Line = null;
				}
			} );
			Disconnect( 300, Easing.Out );

			return 500;
		}

		public void Disconnect ( double duration, Easing easing = Easing.None ) {
			Progress.AnimateATo( 1, duration, easing );
		}

		protected override void Dispose ( bool isDisposing ) {
			if ( isDisposing ) return;

			Line?.Dispose();
			base.Dispose( isDisposing );
		}
	}
}
