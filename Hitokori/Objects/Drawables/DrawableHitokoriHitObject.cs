using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public abstract class DrawableHitokoriHitObject : DrawableHitObject<HitokoriHitObject> {
		public DrawableHitokoriHitObject ( HitokoriHitObject? hitObject = null ) : base( hitObject! ) {
			AutoSizeAxes = Axes.Both;
		}

		protected override double InitialLifetimeOffset => Math.Max( 2000, HitObject is TilePoint tp ? ( tp.FromPrevious is null ? 0 : tp.FromPrevious.Duration ) : 0 );
	}

	public abstract class DrawableHitokoriHitObject<T> : DrawableHitokoriHitObject where T : HitokoriHitObject {
		new public T HitObject => (T)base.HitObject;

		/// <summary>
		/// Whether to set <see cref="Drawable.Position"/> automatically based on the assigned tile.
		/// </summary>
		protected virtual bool ShouldAutoManagePosition => true;

		private BindableFloat positionScale = new( HitokoriPlayfield.DefaultPositionScale );
		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );

			positionScale.BindValueChanged( _ => updatePosition() );
		}

		private void updatePosition () {
			if ( ShouldAutoManagePosition && HitObject is TilePoint tile )
				Position = (Vector2)tile.Position * positionScale.Value;
		}

		protected override void OnApply () {
			if ( HitObject is TilePoint tile ) {
				tile.BindablePosition.BindValueChanged( onTilePositionChanged, true );
			}
			base.OnApply();
		}

		protected override void OnFree () {
			if ( HitObject is TilePoint tile ) {
				tile.BindablePosition.ValueChanged -= onTilePositionChanged;
			}
			base.OnFree();
		}

		private void onTilePositionChanged ( ValueChangedEvent<Vector2d> _ ) {
			updatePosition();
		}
	}
}
