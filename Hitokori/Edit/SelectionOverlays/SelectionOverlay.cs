using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.UI;
using osuTK;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Edit.SelectionOverlays {
	public abstract class SelectionOverlay<T> : CompositeDrawable where T : HitokoriHitObject {
		[Resolved, MaybeNull, NotNull]
		public HitokoriPlayfield Playfield { get; private set; }
		[Resolved, MaybeNull, NotNull]
		public HitokoriBeatmap Beatmap { get; private set; }
		[Resolved, MaybeNull, NotNull]
		public HitokoriHitObjectComposer Composer { get; private set; }

		public readonly T HitObject;

		protected SelectionOverlay ( T hitObject ) {
			HitObject = hitObject;
		}

		public Vector2 PositionOf ( Vector2 normalizedPosition )
			=> ToLocalSpace( Playfield.ScreenSpacePositionOf( normalizedPosition ) );
		public Vector2 PositionOf ( Vector2d normalizedPosition )
			=> PositionOf( (Vector2)normalizedPosition );
		public Vector2 PositionOf ( TilePoint tp )
			=> PositionOf( tp.Position );
	}
}
