using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Scoring {
	public class HitokoriScoreProcessor : ScoreProcessor {
		public override HitWindows CreateHitWindows ()
			=> new HitokoriHitWindows();
	}
}
