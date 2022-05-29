﻿namespace osu.Game.Rulesets.Hitokori.Orbitals.Events {
	public class FadeVisualEvent : VisualEvent<Orbital> {
		public double StartAlpha;
		public double TargetAlpha;

		public FadeVisualEvent ( Orbital target, double targetAlpha, double startTime, double duration = 0, Easing easing = Easing.None ) : base( target, startTime, duration, easing ) {
			TargetAlpha = targetAlpha;
		}

		protected override void Apply ( double progress ) {
			Target.Opacity = (float)(StartAlpha + ( TargetAlpha - StartAlpha ) * progress);
		}

		protected override void OnBegin () {
			StartAlpha = Target.Opacity;
		}

		private static readonly string[] categories = new string[] { CategoryAlpha };
		public override IEnumerable<string> Categories => categories;
	}
}
