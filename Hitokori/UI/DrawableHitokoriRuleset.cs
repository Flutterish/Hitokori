using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class DrawableHitokoriRuleset : DrawableRuleset<HitokoriHitObject> {
		public DrawableHitokoriRuleset ( Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null ) : base( ruleset, beatmap, mods ) {
			
		}

		public override DrawableHitObject<HitokoriHitObject> CreateDrawableRepresentation ( HitokoriHitObject h )
			=> null;

		protected override PassThroughInputManager CreateInputManager ()
			=> new HitokoriInputManager( Ruleset.RulesetInfo, 0, SimultaneousBindingMode.Unique );

		protected override Playfield CreatePlayfield ()
			=> new HitokoriPlayfield();
	}
}
