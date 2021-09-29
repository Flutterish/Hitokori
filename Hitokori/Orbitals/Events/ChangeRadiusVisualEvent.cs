using osu.Framework.Graphics;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Orbitals.Events {
	public class ChangeRadiusVisualEvent : VisualEvent<Orbital> {
		public double TargetRadius;
		public double StartRadius;

		public ChangeRadiusVisualEvent ( Orbital target, double targetRadius, double startTime, double duration = 0, Easing easing = Easing.None ) : base( target, startTime, duration, easing ) {
			TargetRadius = targetRadius;
		}

		protected override void Apply ( double progress ) {
			Target.Radius = StartRadius + ( TargetRadius - StartRadius ) * progress;
		}

		protected override void OnBegin () {
			StartRadius = Target.Radius;
		}

		private static readonly string[] categories = new string[] { CategoryRadius, CategoryPosition };
		public override IEnumerable<string> Categories => categories;
	}
}
