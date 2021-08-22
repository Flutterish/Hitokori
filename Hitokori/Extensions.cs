using osu.Game.Beatmaps;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori {
	public static class Extensions {
		public static float AngleTo ( this Vector2 a, Vector2 b ) {
			var diff = b - a;
			return MathF.Atan2( diff.Y, diff.X );
		}

		public static double AngleTo ( this Vector2d a, Vector2d b ) {
			var diff = b - a;
			return Math.Atan2( diff.Y, diff.X );
		}

		public static float RadToDeg ( this float rad )
			=> rad / MathF.PI * 180;
		public static float DegToRad ( this float deg )
			=> deg * MathF.PI / 180;

		public static double RadToDeg ( this double rad )
			=> rad / Math.PI * 180;
		public static double DegToRad ( this double deg )
			=> deg * Math.PI / 180;

		public static float NormalizeAngle0Tau ( this float angle )
			=> ((angle % MathF.Tau) + MathF.Tau) % MathF.Tau;
		public static double NormalizeAngle0Tau ( this double angle )
			=> ((angle % Math.Tau) + Math.Tau) % Math.Tau;

		public static float NormalizeAnglePiPi ( this float angle )
			=> angle.NormalizeAngle0Tau() - MathF.PI;
		public static double NormalizeAnglePiPi ( this double angle )
			=> angle.NormalizeAngle0Tau() - Math.PI;

		public static Vector2 AngleToVector ( this float angle, float radius = 1 )
			=> new Vector2( MathF.Cos( angle ), MathF.Sin( angle ) ) * radius;
		public static Vector2d AngleToVector ( this double angle, double radius = 1 )
			=> new Vector2d( Math.Cos( angle ), Math.Sin( angle ) ) * radius;

		public static int Mod ( this int i, int mod ) {
			i = i % mod;
			if ( i < 0 ) return i + mod;
			return i;
		}

		public static Vector2 Rotate ( this Vector2 vector, float angle ) {
			var cos = MathF.Cos( angle );
			var sin = MathF.Sin( angle );

			return new Vector2(
				vector.X * cos - vector.Y * sin,
				vector.X * sin + vector.Y * cos
			);
		}

		public static Vector2d Rotate ( this Vector2d vector, double angle ) {
			var cos = Math.Cos( angle );
			var sin = Math.Sin( angle );

			return new Vector2d(
				vector.X * cos - vector.Y * sin,
				vector.X * sin + vector.Y * cos
			);
		}

		public static double BeatLengthAt ( this IBeatmap beatmap, double time )
			=> beatmap.ControlPointInfo.TimingPointAt( time ).BeatLength;
	}
}
