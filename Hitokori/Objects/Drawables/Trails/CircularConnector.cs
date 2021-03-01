using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails {
	public class CircularConnector : Connector {
		private Vector2 from;
		public Vector2 From {
			get => from;
			set {
				if ( from == value ) return;

				from = value;
				isInvalidated = true;
			}
		}
		protected Vector2 Around;
		public double Angle {
			get => angle;
			set {
				if ( angle == value ) return;

				angle = value;
				isInvalidated = true;
			}
		}
		protected double angle;

		public CircularConnector ( Vector2 from, Vector2 around, double angle ) : this() {
			From = from;
			Around = around;
			this.angle = angle;
		}

		CircularProgress arc;

		public CircularConnector () {
			InternalChild = arc = new CircularProgress().Center();
			AutoSizeAxes = Axes.None;
		}

		protected override bool ComputeIsMaskedAway ( RectangleF maskingBounds )
			=> false;

		protected override void render () {
			var radius = TrailRadius + Around.DistanceTo( From );

			arc.Position = Around - From;
			arc.InnerRadius = TrailRadius * 2 / radius;
			arc.Size = new Vector2( radius * 2 );

			arc.FillTo( ( progress.B - progress.A ) / Math.PI / 2 * angle );

			arc.Rotation = ( ( Around - From ).AngleFromXAxis() + ( angle >= 0 ? progress.A : progress.B ) * angle ).ToDegreesF() - 90;
		}
	}

	public class CircularTileConnector : CircularConnector {
		new public TilePoint From;
		new public TilePoint Around;

		public CircularTileConnector () { }
		public CircularTileConnector ( TilePoint from, TilePoint around, double angle ) {
			From = from;
			Around = around;
			this.angle = angle;
		}

		protected override void Update () {
			if ( From is null || Around is null ) {
				base.Update();
				return;
			}
			else if ( From.TilePosition != base.From || Around.TilePosition != base.Around ) {
				base.From = From.TilePosition;
				base.Around = Around.TilePosition;
				isInvalidated = true;
			}

			base.Update();
		}

		BindableDouble width = new( 1 );
		[BackgroundDependencyLoader( true )]
		private void load ( HitokoriSettingsManager config ) {
			config?.BindWith( HitokoriSetting.HoldConnectorWidth, width );
			width.BindValueChanged( v => TrailRadius = HitokoriTile.SIZE / 4f * (float)v.NewValue, true );
		}
	}
}
