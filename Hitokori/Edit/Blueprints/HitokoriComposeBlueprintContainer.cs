using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Hitokori.Edit.Blueprints {
	public class HitokoriComposeBlueprintContainer : ComposeBlueprintContainer {
		new public HitokoriHitObjectComposer Composer => (HitokoriHitObjectComposer)base.Composer;

		public HitokoriComposeBlueprintContainer ( HitokoriHitObjectComposer composer ) : base( composer ) { }

		protected override SelectionHandler<HitObject> CreateSelectionHandler ()
			=> new HitokoriSelectionHandler();

		public override HitObjectSelectionBlueprint CreateHitObjectBlueprintFor ( HitObject hitObject ) {
			if ( hitObject is HitokoriHitObject ho )
				return ho.CreateSelectionBlueprint() ?? base.CreateHitObjectBlueprintFor( hitObject );

			return base.CreateHitObjectBlueprintFor( hitObject );
		}
	}
}
