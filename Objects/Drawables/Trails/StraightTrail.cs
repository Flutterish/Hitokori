using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails {
	public class StraightTrail : TrailRenderer {
		Box box;

		public StraightTrail () {
			InternalChild = box = new Box().Center();
		}

		public StraightTrail ( Vector2 from, Vector2 to ) : this() {
			From = from;
			To = to;
		}

		protected Vector2 From;
		protected Vector2 To;

		protected override void render () {
			var from = ( To - From ) * (float)progress.A;
			var to =   ( To - From ) * (float)progress.B;

			box.Position = ( from + to ) / 2;
			box.Size = new Vector2( 0, from.DistanceTo( to ) ) + new Vector2( TrailRadius * 2 );
			box.Rotation = ( To - From ).AngleFromXAxis().ToDegreesF() + 90;
		}
	}

	public class StraightTileTrail : StraightTrail {
		new public TilePoint From;
		new public TilePoint To;

		public StraightTileTrail ( TilePoint from, TilePoint to ) {
			From = from;
			To = to;
			InternalChild.Alpha = 0.2f;
		}

		protected override void Update () {
			if ( From.TilePosition != base.From || To.TilePosition != base.To ) {
				base.From = From.TilePosition;
				base.To = To.TilePosition;
				isInvalidated = true;
			}
			
			base.Update();
		}

		BindableDouble width = new();
		[BackgroundDependencyLoader]
		private void load ( HitokoriSettingsManager config ) {
			config.BindWith( HitokoriSetting.ConnectorWidth, width );
			width.BindValueChanged( v => TrailRadius = HitokoriTile.SIZE / 8f * (float)v.NewValue, true );
		}
	}
}
