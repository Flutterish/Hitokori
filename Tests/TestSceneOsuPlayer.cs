using NUnit.Framework;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Hitokori.Tests {
	[TestFixture]
	public class TestSceneHitokoriPlayer : PlayerTestScene {
		protected override Ruleset CreatePlayerRuleset () => new HitokoriRuleset();
	}
}
