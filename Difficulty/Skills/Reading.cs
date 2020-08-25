using osu.Framework.Input.Events;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osuTK.Graphics.OpenGL;
using System;

namespace osu.Game.Rulesets.Hitokori.Difficulty.Skills {
	public class Reading : Skill {
		protected override double SkillMultiplier => 1;
		protected override double StrainDecayBase => 0.6;

		protected override double StrainValueOf(DifficultyHitObject current) {
			HitokoriDifficultyHitObject hitokoriCurrent = (HitokoriDifficultyHitObject)current;

			double strain = CalculateAngleStrain(hitokoriCurrent.HitAngle);

			//Hit strain grows by 5% when this object changes the direction
			if (hitokoriCurrent.ChangedDirection) strain *= 1.05;

			if (hitokoriCurrent.HoldAngle != null) {
				strain += CalculateAngleStrain(hitokoriCurrent.HoldAngle.Value);
			}

			return (1 + strain) * 0.6;
		}

		//Strain linearly grows from 0.0 to 2/3 the farther the angle is from 180 degrees
		private double CalculateAngleStrain(double angle) {
			return Math.Abs((Math.Abs(angle) - Math.PI) / (Math.PI * 3 / 2));
        }
	}
}
