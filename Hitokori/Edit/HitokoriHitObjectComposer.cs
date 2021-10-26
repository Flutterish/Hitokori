using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Hitokori.Objects;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Edit {
	public class HitokoriHitObjectComposer : HitObjectComposer<HitokoriHitObject> {
		public HitokoriHitObjectComposer ( Ruleset ruleset ) : base( ruleset ) { }

		protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => Array.Empty<HitObjectCompositionTool>();
	}
}
