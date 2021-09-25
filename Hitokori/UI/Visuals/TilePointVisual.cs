using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Objects.Drawables;
using System.Linq;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.UI.Visuals {
	public class TilePointVisual : AppliableVisual<TilePoint> {
		TilePointVisualPiece piece;

		public TilePointVisual () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AddInternal( piece = new TilePointVisualPiece() );
		}

		protected override void OnApply ( TilePoint hitObject ) {
			base.OnApply( hitObject );

			if ( hitObject.FromPrevious is IHasVelocity fromv && hitObject.ToNext is IHasVelocity tov ) {
				if ( fromv.Speed / tov.Speed < 0.95 ) {
					piece.Colour = Colour4.Tomato;
				}
				else if ( tov.Speed / fromv.Speed < 0.95 ) {
					piece.Colour = Colour4.LightBlue;
				}
				else {
					piece.Colour = Colour4.HotPink;
				}
			}
		}

		public override void UpdateInitialTransforms () {
			if ( AppliedHitObject!.Next is not null ) {
				this.TransformBindableTo( piece.OutAnimationProgress, 0 );
				using ( BeginAbsoluteSequence( AppliedHitObject.Next.StartTime - 2000 ) ) {
					this.TransformBindableTo( piece.OutAnimationProgress, 1, 750, Easing.Out );
				}
			}

			if ( AppliedHitObject.Previous is not null ) {
				this.TransformBindableTo( piece.InAnimationProgress, 0 );
				using ( BeginAbsoluteSequence( AppliedHitObject.StartTime - 2000 ) ) {
					this.TransformBindableTo( piece.InAnimationProgress, 1, 750, Easing.Out );
				}
			}

			piece.ScaleTo( 0 ).Then().ScaleTo( 1, 300, Easing.Out );
		}

		public override void UpdateHitStateTransforms ( ArmedState state ) {
			var fadeOutDuration = 300;
			var fadeOutEasing = Easing.In;

			if ( state is ArmedState.Hit or ArmedState.Miss ) {
				piece.LightUp();

				var nextSwapTile = AppliedHitObject!.AllNext.FirstOrDefault( x => x.OrbitalState.ActiveIndex != AppliedHitObject.OrbitalState.ActiveIndex );
				using ( BeginAbsoluteSequence( nextSwapTile?.StartTime ?? LatestTransformEndTime ) ) {
					piece.ScaleTo( 0, fadeOutDuration, fadeOutEasing );
				}
			}
		}

		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, piece.PositionScale );
		}

		protected override void Update () {
			if ( !IsApplied ) return;

			piece.FromPosition.Value = AppliedHitObject.Previous?.Position;
			piece.ToPosition.Value = AppliedHitObject.Next?.Position;
			piece.AroundPosition.Value = AppliedHitObject.Position;
		}

		public override double EndTime => piece.LatestTransformEndTime;
	}
}
