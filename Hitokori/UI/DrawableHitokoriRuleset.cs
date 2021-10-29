using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Hitokori.Camera;
using osu.Game.Rulesets.Hitokori.Edit;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class DrawableHitokoriRuleset : DrawableRuleset<HitokoriHitObject> {
		public DrawableHitokoriRuleset ( Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null ) : base( ruleset, beatmap, mods ) { }

		public bool IsEditor = false;

		public override DrawableHitObject<HitokoriHitObject>? CreateDrawableRepresentation ( HitokoriHitObject h )
			=> null;

		protected override PassThroughInputManager CreateInputManager ()
			=> new HitokoriInputManager( Ruleset.RulesetInfo, 0, SimultaneousBindingMode.Unique );
		protected override ReplayInputHandler CreateReplayInputHandler ( Replay replay )
			=> new HitokoriReplayInputHandler( replay );
		protected override ReplayRecorder CreateReplayRecorder ( Score score )
			=> new HitokoriReplayRecorder( score );

		protected override Playfield CreatePlayfield ()
			=> IsEditor
			? new HitokoriEditorPlayfield( Beatmap )
			: new HitokoriPlayfield( Beatmap, new RegularCameraPathGenerator ( Beatmap ).GenerateEasedPath () );
	}
}
