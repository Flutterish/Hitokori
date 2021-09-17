using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using System;

namespace osu.Game.Rulesets.Hitokori.Difficulty.Skills {
	public class Speed : StrainSkill {
		protected override double DecayWeight => 0.6;

		/// <summary>
		/// The BPM where strain stops increasing
		/// </summary>
		private const double MAX_BPM = 300;

		/// <summary>
		/// The BPM where strain is 1.0
		/// </summary>
		private const double BASE_BPM = 120;

		public Speed ( Mod[] mods ) : base( mods ) { }

		protected override double StrainValueAt ( DifficultyHitObject current ) {
			HitokoriDifficultyHitObject hitokoriCurrent = (HitokoriDifficultyHitObject)current;
			double bpm = Math.Min( hitokoriCurrent.BPM, MAX_BPM );

			return Math.Pow( bpm / BASE_BPM, 0.6 );
		}

		protected override double CalculateInitialStrain ( double time ) => 0;
	}
}
