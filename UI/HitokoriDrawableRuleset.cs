using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Hitokori.Mods;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class HitokoriDrawableRuleset : DrawableRuleset<HitokoriHitObject> {
		public HitokoriDrawableRuleset ( Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null ) : base( ruleset, beatmap, mods ) { }

		public override DrawableHitObject<HitokoriHitObject> CreateDrawableRepresentation ( HitokoriHitObject h )
			=> h.AsDrawable();

		protected override PassThroughInputManager CreateInputManager ()
			=> new HitokoriInputManager( Ruleset.RulesetInfo, 0, Framework.Input.Bindings.SimultaneousBindingMode.Unique );

		protected override ReplayInputHandler CreateReplayInputHandler ( Replay replay )
			=> new HitokoriReplayInputHandler( replay );

		protected override Playfield CreatePlayfield ()
			=> new HitokoriPlayfield( Mods.Any( x => x is HitokoriModAuto ), Mods.Any( X => X is HitokoriModTriplets ) );

		protected override ReplayRecorder CreateReplayRecorder ( Replay replay )
			=> new HitokoriReplayRecorder( replay );
	}
}
