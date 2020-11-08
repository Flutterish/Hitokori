using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails {
	/// <summary>
	/// A connector is a path between 2 tiles. Its center position is offset by whatever the offset is from the "around" position, that is
	/// if you set its position to `<c>around - to</c>` its start will be centered at "around";
	/// </summary>
	public class ArchedPathTileConnector : ArchedPathConnector {
		new TilePoint Around;
		new TilePoint From;
		new TilePoint To;

		public ArchedPathTileConnector ( TilePoint from, TilePoint to, TilePoint around, double angle, float alpha = 0.2F ) : base( angle, alpha ) {
			Around = around;
			From = from;
			To = to;
		}

		protected override void Update () {
			base.From = From.TilePosition;
			base.To = To.TilePosition;
			base.Around = Around.TilePosition;
			base.Update();
		}

		BindableDouble width = new();
		[BackgroundDependencyLoader]
		private void load ( HitokoriSettingsManager config ) {
			config.BindWith( HitokoriSetting.HoldConnectorWidth, width );
			width.BindValueChanged( v => LineRadius = HitokoriTile.SIZE / 4f * (float)width.Value, true );
		}
	}

	public class ArchedPathConnector : PathConnector { // TODO on miss connectors
		public Vector2 Around;
		double Angle;

		public ArchedPathConnector ( double angle, float alpha = 0.2F ) : base( alpha ) {
			Angle = angle;
		}

		protected override IEnumerable<Vector2> getVerticesAt ( float progressA, float progressB ) {
			var deltaFrom = From - Around;

			double from = Math.Atan2( deltaFrom.Y, deltaFrom.X );
			double to = from + Angle;
			double deltaAngle = to - from;

			to = from + progressB * deltaAngle;
			from += progressA * deltaAngle;

			double startDistance = ( From - Around ).Length;
			double endDistance = ( To - Around ).Length;
			double startAngle = from;
			if ( from > to ) {
				Utils.Extensions.Swap( ref from, ref to );
			}

			deltaAngle = to - from;

			if ( deltaAngle < 0.001 ) {
				yield break;
			}

			int steps = (int)Math.Max( deltaAngle / ( Math.PI / 64 ), 2 );

			for ( int i = 0; i < steps; i++ ) {
				var angle = from + deltaAngle / ( steps - 1 ) * i;
				var distance = startDistance + ( endDistance - startDistance ) * ( angle - startAngle ) / Angle;

				yield return new Vector2( (float)Math.Cos( angle ), (float)Math.Sin( angle ) ) * (float)distance;
			}
		}

		public double Appear ( double duration ) {
			this.FadeInFromZero( 700, Easing.Out );
			Connect( duration );

			return Math.Max( 700, duration );
		}

		public double Disappear ( double duration ) {
			Disconnect( duration );

			return duration + 200;
		}
	}
}
