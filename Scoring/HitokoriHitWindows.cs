using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Scoring {
	public class HitokoriHitWindows : HitWindows {
		public override bool IsHitResultAllowed ( HitResult result ) {
			return result switch
			{
				HitResult.Miss => true,
				HitResult.Meh => true,
				HitResult.Good => true,
				HitResult.Perfect => true,

				_ => false
			};
		}

		protected override DifficultyRange[] GetRanges ()
			=> new DifficultyRange[] {
				new DifficultyRange( HitResult.Perfect, 80  * MOD, 50  * MOD, 20  * MOD ),
				new DifficultyRange( HitResult.Good,    140 * MOD, 100 * MOD, 60  * MOD ),
				new DifficultyRange( HitResult.Meh,     200 * MOD, 150 * MOD, 100 * MOD ),
				new DifficultyRange( HitResult.Miss,    300 * MOD, 250 * MOD, 200 * MOD )
			};

		const double MOD = 0.5;
	}

	public class HitokoriAngleHitWindows : HitWindows {
		public override bool IsHitResultAllowed ( HitResult result ) {
			return result switch
			{
				HitResult.Miss => true,
				HitResult.Meh => true,
				HitResult.Good => true,
				HitResult.Perfect => true,

				_ => false
			};
		}

		protected override DifficultyRange[] GetRanges () // TODO figure out these fuckers
			=> new DifficultyRange[] {
				new DifficultyRange( HitResult.Perfect, 8  * MOD, 7  * MOD, 5  * MOD ),
				new DifficultyRange( HitResult.Good,    10 * MOD, 9  * MOD, 8  * MOD ),
				new DifficultyRange( HitResult.Meh,     12 * MOD, 11 * MOD, 10 * MOD ),
				new DifficultyRange( HitResult.Miss,    14 * MOD, 13 * MOD, 12 * MOD )
			};

		const double MOD = 1;
	}
}
