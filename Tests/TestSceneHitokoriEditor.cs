using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Hitokori.Tests {
	public class TestSceneHitokoriEditor : EditorTestScene {
		protected override Ruleset CreateEditorRuleset () => new HitokoriRuleset();
	}
}
