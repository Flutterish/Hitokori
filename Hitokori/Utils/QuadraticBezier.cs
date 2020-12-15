using osuTK;

namespace osu.Game.Rulesets.Hitokori.Utils {
	public struct QuadraticBezier {
		public Vector2 Start;
		public Vector2 Control;
		public Vector2 End;

		public QuadraticBezier ( Vector2 start, Vector2 control, Vector2 end ) {
			Start = start;
			Control = control;
			End = end;
		}

		public Vector2 Evaluate ( float time ) {
			Vector2 A = Start + ( Control - Start ) * time;
			Vector2 B = Control + ( End - Control ) * time;

			return A + ( B - A ) * time;
		}
	}
}
