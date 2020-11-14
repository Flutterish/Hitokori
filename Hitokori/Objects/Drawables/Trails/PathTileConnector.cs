using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails {
	/// <summary>
	/// A connector is a path between 2 tiles. Its start position is offset by whatever the offset is from the "from" tile, that is
	/// if you set its position to `<c>from.TilePosition - to.TilePosition</c>` it will be centered at "from";
	/// </summary>
	public class PathTileConnector : PathConnector {
		new public TilePoint From;
		new public TilePoint To;

		public PathTileConnector ( TilePoint from, TilePoint to, float alpha = 0.2f ) : base( alpha ) {
			From = from;
			To = to;
		}

		protected override void Update () {
			base.From = From.TilePosition;
			base.To = To.TilePosition;

			base.Update();
		}

		BindableDouble width = new( 1 );
		[BackgroundDependencyLoader(true)]
		private void load ( HitokoriSettingsManager config ) {
			config?.BindWith( HitokoriSetting.ConnectorWidth, width );
			width.BindValueChanged( v => LineRadius = HitokoriTile.SIZE / 8f * (float)width.Value, true );
		}
	}

	public class PathConnector : Container {
		[Resolved]
		private PathPool pathPool { get; set; }

		private PooledPath Line;

		private float lineRadius = HitokoriTile.SIZE / 8f;
		public float LineRadius {
			get => lineRadius;
			set {
				if ( lineRadius == value ) return;

				lineRadius = value;
				if ( Line is not null ) Line.PathRadius = lineRadius;
			}
		}
		private double alpha = 0.2f;
		public double LineAlpha {
			get => alpha;
			set {
				alpha = value;

				if ( Line is not null ) Line.Alpha = (float)LineAlpha;
			}
		}

		public Vector2 From;
		public Vector2 To;

		protected AnimatedVector Progress;

		public PathConnector ( float alpha = 0.2f ) {
			Progress = new AnimatedVector( parent: this );

			LineAlpha = alpha;
			this.Center();
		}

		protected override void Update () {
			if ( Math.Abs( Progress.A - Progress.B ) < 0.01 ) {
				if ( Line != null ) {
					Line.Release();
					Line = null;
				}
			}
			else if ( Line is null ) {
				InternalChild = Line = pathPool.Borrow( getMaxRequiredSize() );
				Line.PathRadius = lineRadius;
				Line.Alpha = (float)LineAlpha;
				Line.Anchor = Anchor.TopLeft;
				Line.Origin = Anchor.TopLeft;
				AutoSizeAxes = Axes.None;
				Line.AutoSizeAxes = Axes.None;
			}

			if ( Line != null ) {
				setLineSize();
				UpdateConnector();
			}
		}

		private void setLineSize () {
			var requiredSize = getMaxRequiredSize();
			if ( requiredSize.X > Line.Size.X || requiredSize.Y > Line.Size.Y ) {
				Line.Size = new Vector2( MathF.Max( requiredSize.X, Line.Size.X ), MathF.Max( requiredSize.Y, Line.Size.Y ) );
				//Size = requiredSize;
			}
		}

		protected void UpdateConnector () {
			Line.ClearVertices();

			foreach ( var i in getVerticesAt( (float)Progress.A, (float)Progress.B ) ) Line.AddVertex( i );

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

		private Vector2 getMaxRequiredSize () {
			var points = getVerticesAt( 0, 1 );
			if ( !points.Any() ) return Vector2.One;
			return new Vector2(
				points.Max( v => v.X ) - points.Min( v => v.X ),
				points.Max( v => v.Y ) - points.Min( v => v.Y )
			) + new Vector2( lineRadius * 2 );
		}

		protected virtual IEnumerable<Vector2> getVerticesAt ( float progressA, float progressB ) {
			yield return ( To - From ) * progressA;
			yield return ( To - From ) * progressB;
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
		public PathPool () {
			while ( paths.Count < 15 ) {
				var @new = new PooledPath {
					AutoSizeAxes = Axes.None,
					Size = new Vector2( 200 )
				};
				paths.Add( @new );
			}
			while ( paths.Count < 25 ) {
				var @new = new PooledPath {
					AutoSizeAxes = Axes.None,
					Size = new Vector2( 250 )
				};
				paths.Add( @new );
			}
		}

		public PooledPath Borrow ( Vector2 targetSize ) {
			PooledPath picked = null;
			for ( int i = 0; i < paths.Count; i++ ) {
				var p = paths[ i ];
				if ( !p.IsBorrowed ) {
					if ( picked is null ) {
						if ( targetSize.FitsInside( p.Size ) ) picked = p;
						continue;
					}
					if ( picked.Size.Area() > p.Size.Area() ) picked = p;
				}
			}

			if ( picked is not null ) {
				picked.Borrow();
				return picked;
			}


			for ( int i = 0; i < paths.Count; i++ ) {
				var p = paths[ i ];
				if ( !p.IsBorrowed ) {
					p.Borrow();
					return p;
				}
			}

			var @new = new PooledPath();
			@new.AutoSizeAxes = Axes.None;
			@new.Size = targetSize;
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
