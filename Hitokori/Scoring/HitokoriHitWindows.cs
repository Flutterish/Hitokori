using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Scoring {
	public class HitokoriHitWindows : HitWindows {
		public override bool IsHitResultAllowed ( HitResult result ) {
			return result switch
			{
				HitResult.Miss => true,
				HitResult.Good => true,
				HitResult.Perfect => true,

				_ => false
			};
		}

		protected override DifficultyRange[] GetRanges ()
			=> new DifficultyRange[] {
				new DifficultyRange( HitResult.Perfect, 70, 50, 40 ),
				new DifficultyRange( HitResult.Good,    200, 150, 100 ),
				new DifficultyRange( HitResult.Miss,    200, 150, 100 )
			};
	}

	public class HitokoriAngleHitWindows : HitWindows {
		public override bool IsHitResultAllowed ( HitResult result ) {
			return result switch
			{
				HitResult.Miss => true,
				HitResult.Good => true,
				HitResult.Perfect => true,

				_ => false
			};
		}

		protected override DifficultyRange[] GetRanges ()
			=> new DifficultyRange[] {
				new DifficultyRange( HitResult.Perfect, 15, 13, 10 ),
				new DifficultyRange( HitResult.Good,    40, 35, 25 ),
				new DifficultyRange( HitResult.Miss,    40, 35, 25 )
			};
	}
}
