using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Hitokori.Objects {
	public abstract class HitokoriHitObject : HitObject {
		public virtual HitObjectSelectionBlueprint? CreateSelectionBlueprint () => null;
	}
}
