using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Timing;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	/// <summary>
	/// A connector is a path between 2 tiles. Its start position is offset by whatever the offset is from the "from" tile, that is
	/// if you set its position to `<c>from.HoldPosition - parent.Position</c>` its start will be centered at "from";
	/// </summary>
	public class Connector : Container {
		public TilePoint From;
		public TilePoint To;

		[Resolved]
		private PathPool pathPool { get; set; }

		public PooledPath Line;

		new public double Alpha;

		protected AnimatedVector Progress;

		public Connector ( TilePoint from, TilePoint to, float alpha = 0.2f ) {
			Progress = new AnimatedVector( parent: this );
			Alpha = alpha;
			this.Center();

			From = from;
			To = to;

			Progress.ValueChanged += x => {
				if ( Line != null ) {
					UpdateConnector();
				}
			};
		}

		protected override void Update () {
			if ( Math.Abs( Progress.A - Progress.B ) < 0.01 ) {
				if ( Line != null ) {
					Line.Release();
					Line = null;
				}
			} else if ( Line is null ) {
				InternalChild = Line = pathPool.Borrow();
				Line.PathRadius = HitokoriTile.SIZE / 8f;
				Line.Alpha = (float)Alpha;
				Line.Anchor = Anchor.TopLeft;
				Line.Origin = Anchor.TopLeft;
			}
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
			this.FadeOut( 300 );
			Disconnect( 300, Easing.Out );

			return 500;
		}

		public void Disconnect ( double duration, Easing easing = Easing.None ) {
			Progress.AnimateATo( 1, duration, easing );
		}

		protected override void Dispose ( bool isDisposing ) {
			Line?.Dispose();

			base.Dispose( isDisposing );
		}
	}

	public class PooledPath : Path {
		public bool IsBorrowed { get; private set; }
		public void Release () {
			IsBorrowed = false;
			( Parent as Container )?.Remove( this );
		}

		public void Borrow () {
			IsBorrowed = true;
		}
	}

	public class PathPool : IDisposable {
		private List<PooledPath> paths = new List<PooledPath>();

		public PooledPath Borrow () {
			foreach ( var path in paths ) {
				if ( !path.IsBorrowed ) {
					path.Borrow();

					return path;
				}
			}

			var @new = new PooledPath();
			@new.Borrow();

			paths.Add( @new );

			return @new;
		}

		public void Dispose () {
			foreach ( var path in paths ) {
				path?.Dispose();
			}
		}
	}
}
