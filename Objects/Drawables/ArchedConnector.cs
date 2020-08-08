using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	/// <summary>
	/// A connector is a path between 2 tiles. Its center position is offset by whatever the offset is from the "around" position, that is
	/// if you set its position to `<c>around - parent.Position</c>` its start will be centered at "around";
	/// </summary>
	public class ArchedConnector : Connector { // TODO on miss connectors
		TilePoint Around;
		double Angle;

		public ArchedConnector ( TilePoint from, TilePoint to, TilePoint around, double angle, float alpha = 0.2F ) : base( from, to, alpha ) {
			Around = around;
			Angle = angle;
		}

		protected override void UpdateConnector () {
			Line.ClearVertices();

			var deltaFrom = From.TilePosition - Around.TilePosition;

			double from = Math.Atan2( deltaFrom.Y, deltaFrom.X );
			double to = from + Angle;
			double deltaAngle = to - from;

			to = from + Progress.B * deltaAngle;
			from += Progress.A * deltaAngle;

			double startDistance = From.Distance;
			double endDistance = To.Distance;
			double startAngle = from;
			if ( from > to ) {
				Utils.Extensions.Swap( ref from, ref to );
			}

			deltaAngle = to - from;

			if ( ( deltaAngle < 0.001 ) ) {
				Line.Alpha = 0;
				return;
			}

			int steps = (int)Math.Max( deltaAngle / ( Math.PI / 32 ), 2 );

			for ( int i = 0; i < steps; i++ ) {
				var angle = from + deltaAngle / ( steps - 1 ) * i;
				var distance = startDistance + ( endDistance - startDistance ) * ( angle - startAngle ) / Angle;

				Line.AddVertex( new Vector2( (float)Math.Cos( angle ), (float)Math.Sin( angle ) ) * HitokoriTile.SPACING * (float)distance );
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
