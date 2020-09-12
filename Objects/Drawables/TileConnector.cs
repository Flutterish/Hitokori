using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Timing;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	/// <summary>
	/// A connector is a path between 2 tiles. Its start position is offset by whatever the offset is from the "from" tile, that is
	/// if you set its position to `<c>from.TilePosition - to.TilePosition</c>` it will be centered at "from";
	/// </summary>
	public class TileConnector : Connector {
		new public TilePoint From;
		new public TilePoint To;

		public TileConnector ( TilePoint from, TilePoint to, float alpha = 0.2f ) : base( alpha ) {
			From = from;
			To = to;
		}

		protected override void UpdateConnector () {
			LineRadius = HitokoriTile.SIZE / 8f * (float)( width?.Value ?? 1 );

			base.From = From.TilePosition;
			base.To = To.TilePosition;
			base.UpdateConnector();
		}

		Bindable<double> width;
		[BackgroundDependencyLoader]
		private void load ( HitokoriSettingsManager config ) {
			width = config.GetBindable<double>( HitokoriSetting.ConnectorWidth );
		}
	}

	public class Connector : Container {
		[Resolved]
		private PathPool pathPool { get; set; }

		public PooledPath Line;

		private float lineRadius = HitokoriTile.SIZE / 8f;
		public float LineRadius {
			get => lineRadius;
			set {
				if ( lineRadius == value ) return;
				lineRadius = value;
				if ( Line is not null ) Line.PathRadius = lineRadius;
			}
		}
		new public double Alpha = 0.2f;

		public Vector2 From;
		public Vector2 To;

		protected AnimatedVector Progress;

		public Connector ( float alpha = 0.2f ) {
			Progress = new AnimatedVector( parent: this );
			Alpha = alpha;
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
				InternalChild = Line = pathPool.Borrow();
				Line.PathRadius = lineRadius;
				Line.Alpha = (float)Alpha;
				Line.Anchor = Anchor.TopLeft;
				Line.Origin = Anchor.TopLeft;
			}

			if ( Line != null ) {
				UpdateConnector();
			}
		}

		protected virtual void UpdateConnector () {
			Line.ClearVertices();

			Line.AddVertex( ( To - From ) * (float)Progress.A );
			Line.AddVertex( ( To - From ) * (float)Progress.B );

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
