using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	/// <summary>
	/// A connector is a path between 2 tiles. Its center position is offset by whatever the offset is from the "around" position, that is
	/// if you set its position to `<c>around - to</c>` its start will be centered at "around";
	/// </summary>
	public class ArchedTileConnector : ArchedConnector { // TODO on miss connectors
		new TilePoint Around;
		new TilePoint From;
		new TilePoint To;

		public ArchedTileConnector ( TilePoint from, TilePoint to, TilePoint around, double angle, float alpha = 0.2F ) : base( angle, alpha ) {
			Around = around;
			From = from;
			To = to;
		}

		protected override void UpdateConnector () {
			base.From = From.TilePosition;
			base.To = To.TilePosition;
			base.Around = Around.TilePosition;
			base.UpdateConnector();
		}
	}

	public class ArchedConnector : Connector { // TODO on miss connectors
		public Vector2 Around;
		double Angle;

		public ArchedConnector ( double angle, float alpha = 0.2F ) : base( alpha ) {
			Angle = angle;
		}

		protected override void UpdateConnector () {
			Line.ClearVertices();

			var deltaFrom = From - Around;

			double from = Math.Atan2( deltaFrom.Y, deltaFrom.X );
			double to = from + Angle;
			double deltaAngle = to - from;

			to = from + Progress.B * deltaAngle;
			from += Progress.A * deltaAngle;

			double startDistance = ( From - Around ).Length;
			double endDistance = ( To - Around ).Length;
			double startAngle = from;
			if ( from > to ) {
				Utils.Extensions.Swap( ref from, ref to );
			}

			deltaAngle = to - from;

			if ( ( deltaAngle < 0.001 ) ) {
				Line.Alpha = 0;
				return;
			}

			int steps = (int)Math.Max( deltaAngle / ( Math.PI / 64 ), 2 );

			for ( int i = 0; i < steps; i++ ) {
				var angle = from + deltaAngle / ( steps - 1 ) * i;
				var distance = startDistance + ( endDistance - startDistance ) * ( angle - startAngle ) / Angle;

				Line.AddVertex( new Vector2( (float)Math.Cos( angle ), (float)Math.Sin( angle ) ) * (float)distance );
			}

			Line.Position = -Line.PositionInBoundingBox( Vector2.Zero ); ;

			Line.Alpha = (float)Alpha;
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
