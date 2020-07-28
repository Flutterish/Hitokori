using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Hitokori {
	public class HitokoriInputManager : RulesetInputManager<HitokoriAction> {
		public HitokoriInputManager ( RulesetInfo ruleset, int variant, SimultaneousBindingMode unique ) : base( ruleset, variant, unique ) { }
	}
}
