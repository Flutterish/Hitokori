using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Hitokori.Input {
	public class HitokoriInputManager : RulesetInputManager<HitokoriAction> {
		public HitokoriInputManager ( RulesetInfo ruleset, int variant, SimultaneousBindingMode unique ) : base( ruleset, variant, unique ) {
		}
	}
}
