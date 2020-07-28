using osu.Game.Beatmaps;
using System;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmapProcessor : BeatmapProcessor {
		public HitokoriBeatmapProcessor ( IBeatmap beatmap ) : base( beatmap ) { }

		public override void PostProcess () {

		}
	}

	public enum Shape {
		Triangle,
		Square,
		Pentagon,
		Hexagon
	}

	public static class ShapeMethods {
		public static double Beats ( this Shape shape ) {
			return shape switch
			{
				Shape.Triangle => 0.33,
				Shape.Square => 0.25,
				Shape.Pentagon => 0.2,
				Shape.Hexagon => 0.16,

				_ => throw new NotImplementedException()
			};
		}

		public static double Angle ( this Shape shape ) {
			return shape switch
			{
				Shape.Triangle => Math.PI / 3,
				Shape.Square => Math.PI / 2,
				Shape.Pentagon => Math.PI * 3 / 5,
				Shape.Hexagon => Math.PI * 2 / 3,

				_ => throw new NotImplementedException()
			};
		}
	}
}
