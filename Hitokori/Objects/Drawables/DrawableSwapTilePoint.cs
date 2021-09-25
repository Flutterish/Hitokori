using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawableSwapTilePoint : DrawableTilePointWithConnections<SwapTilePoint>, IKeyBindingHandler<HitokoriAction> {
		public DrawableSwapTilePoint () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;
		}

		private BindableFloat positionScale = new( HitokoriPlayfield.DefaultPositionScale );
		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );
		}

		protected override void OnApply () {
			base.OnApply();
			Position = (Vector2)HitObject.Position * positionScale.Value;
		}

		protected override void Update () {
			base.Update();

			Position = (Vector2)HitObject.Position * positionScale.Value;
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( userTriggered ) {
				var result = HitObject.HitWindows.ResultFor( timeOffset );

				if ( result is HitResult.None ) {
					// TODO some animation to show the "too early"
				}
				else {
					ApplyResult( j => j.Type = result );
				}
			}
			else if ( !HitObject.HitWindows.CanBeHit( timeOffset ) ) {
				ApplyResult( j => j.Type = HitResult.Miss );
			}
		}

		public bool OnPressed ( KeyBindingPressEvent<HitokoriAction> action ) {
			if ( Judged ) return false;

			UpdateResult( true );
			return true;
		}

		public void OnReleased ( KeyBindingReleaseEvent<HitokoriAction> action ) { }

		protected override double InitialLifetimeOffset => 2000;
	}
}
