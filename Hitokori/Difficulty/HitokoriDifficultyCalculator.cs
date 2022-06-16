using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Hitokori.Difficulty.Skills;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Difficulty {
	public class HitokoriDifficultyCalculator : DifficultyCalculator {
		private const double STAR_SCALING_FACTOR = 0.04125; //Taken from Taiko's difficulty calculations

		public HitokoriDifficultyCalculator ( IRulesetInfo ruleset, IWorkingBeatmap beatmap ) : base( ruleset, beatmap ) { }

		protected override DifficultyAttributes CreateDifficultyAttributes ( IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate ) {
			// BUG these are faulty and return values around 0.14*
			//double readingDifficulty = skills.First<Reading>().DifficultyValue() * STAR_SCALING_FACTOR;
			//double speedDifficulty = skills.First<Speed>().DifficultyValue() * STAR_SCALING_FACTOR;
			//double starRating = ( readingDifficulty + speedDifficulty ) / 2;

			return new DifficultyAttributes {
				StarRating = beatmap.Difficulty.OverallDifficulty,
				Mods = mods
			};
		}

		protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects ( IBeatmap beatmap, double clockRate ) {
			HitObject last = beatmap.HitObjects.First();
			List<DifficultyHitObject> objects = new();
			int index = 0;
			foreach ( HitObject hitObject in beatmap.HitObjects.Skip( 1 ) ) {
				TimingControlPoint timingControlPoint = beatmap.ControlPointInfo.TimingPointAt( hitObject.StartTime );
				objects.Add( new HitokoriDifficultyHitObject( hitObject, last, timingControlPoint.BPM, clockRate, objects, index++ ) );
				last = hitObject;
			}

			return objects;
		}

		protected override Skill[] CreateSkills ( IBeatmap beatmap, Mod[] mods, double clockRate ) {
			return new Skill[] { new Reading( mods ), new Speed( mods ) };
		}
	}
}
