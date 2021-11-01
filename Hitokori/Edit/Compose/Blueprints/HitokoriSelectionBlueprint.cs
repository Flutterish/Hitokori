using osu.Framework.Allocation;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.UI;
using osuTK;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose.Blueprints {
	public abstract class HitokoriSelectionBlueprint<T> : HitObjectSelectionBlueprint<T> where T : HitokoriHitObject {
		[Resolved, MaybeNull, NotNull]
		public HitokoriPlayfield Playfield { get; private set; }
		[Resolved, MaybeNull, NotNull]
		public HitokoriBeatmap Beatmap { get; private set; }

		protected HitokoriSelectionBlueprint ( T item ) : base( item ) { }

		protected override bool AlwaysShowWhenSelected => true;

		public Vector2 PositionOf ( Vector2 normalizedPosition )
			=> ToLocalSpace( Playfield.ScreenSpacePositionOf( normalizedPosition ) );
		public Vector2 PositionOf ( Vector2d normalizedPosition )
			=> PositionOf( (Vector2)normalizedPosition );
		public Vector2 PositionOf ( TilePoint tp )
			=> PositionOf( tp.Position );
	}
}
