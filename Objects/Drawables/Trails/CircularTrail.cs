using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails {
	public class CircularTrail : TrailRenderer {
		protected Vector2 From;
		protected Vector2 Around;
		protected double angle;

		public CircularTrail ( Vector2 from, Vector2 around, double angle ) : this() {
			From = from;
			Around = around;
			this.angle = angle;
		}

		CircularProgress arc;
		public CircularTrail () {
			InternalChild = arc = new CircularProgress().Center();
			AutoSizeAxes = Axes.None;
		}

		protected override void render () {
			var radius = TrailRadius + Around.DistanceTo( From );

			arc.Position = Around - From;
			arc.InnerRadius = TrailRadius * 2 / radius;
			arc.Size = new Vector2( radius * 2 );

			if ( angle >= 0 ) {
				arc.FillTo( ( progress.B - progress.A ) / Math.PI / 2 * angle );
				arc.Rotation = ( ( Around - From ).AngleFromXAxis() + progress.A * angle ).ToDegreesF() - 90;
			}
			else {
				arc.FillTo( ( progress.A - progress.B ) / Math.PI / 2 * -angle );
				arc.Rotation = ( ( Around - From ).AngleFromXAxis() + progress.B * angle ).ToDegreesF() - 90;
			}
		}
	}

	public class CircularTileTrail : CircularTrail {
		new public TilePoint From;
		new public TilePoint Around;

		public CircularTileTrail ( TilePoint from, TilePoint around, double angle ) {
			From = from;
			Around = around;
			this.angle = angle;
		}

		protected override void Update () {
			if ( From.TilePosition != base.From || Around.TilePosition != base.Around ) {
				base.From = From.TilePosition;
				base.Around = Around.TilePosition;
				isInvalidated = true;
			}

			base.Update();
		}

		BindableDouble width = new();
		[BackgroundDependencyLoader]
		private void load ( HitokoriSettingsManager config ) {
			config.BindWith( HitokoriSetting.HoldConnectorWidth, width );
			width.BindValueChanged( v => TrailRadius = HitokoriTile.SIZE / 4f * (float)v.NewValue, true );
		}
	}
}
