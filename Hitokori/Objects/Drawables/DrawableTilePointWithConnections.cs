﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI.Visuals;
using osu.Game.Rulesets.Objects.Drawables;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawableTilePointWithConnections<T> : DrawableHitokoriHitObject<T> where T : TilePoint {
		TilePointVisualPiece piece;

		public DrawableTilePointWithConnections () {
			AddInternal( piece = new TilePointVisualPiece() );
		}

		public override void UpdateInitialVisuals () {
			RefreshStateTransforms();
			base.UpdateInitialVisuals();

			piece.BorderColour = Colour4.White;
			if ( HitObject.FromPrevious is IHasVelocity fromv && HitObject.ToNext is IHasVelocity tov ) {
				if ( fromv.Speed / tov.Speed < 0.95 ) {
					piece.Colour = Colour4.Tomato;
				}
				else if ( tov.Speed / fromv.Speed < 0.95 ) {
					piece.Colour = Colour4.LightBlue;
				}
				else {
					piece.Colour = Colour4.HotPink;
				}

				if ( Math.Sign( fromv.Velocity ) != Math.Sign( tov.Velocity ) ) {
					piece.BorderColour = Colour4.Yellow;
				}
			}
		}

		protected override void UpdateInitialTransforms () {
			if ( HitObject.ToNext is not null ) {
				this.TransformBindableTo( piece.OutAnimationProgress, 0 );
				using ( BeginAbsoluteSequence( joinTime( HitObject.ToNext, -HitObject.MinumumLifetimeOffset ) ) ) {
					this.TransformBindableTo( piece.OutAnimationProgress, 1, 750, Easing.Out );
				}
			}

			if ( HitObject.FromPrevious is not null ) {
				this.TransformBindableTo( piece.InAnimationProgress, 0 );
				using ( BeginAbsoluteSequence( joinTime( HitObject.FromPrevious, -HitObject.Previous!.MinumumLifetimeOffset ) ) ) {
					this.TransformBindableTo( piece.InAnimationProgress, 1, 750, Easing.Out );
				}
			}

			piece.ScaleTo( 0 ).Then().ScaleTo( 1, 300, Easing.Out );
		}

		private double joinTime ( TilePointConnector c, double offset = -2000 ) {
			return Math.Min( 
				c.To.StartTime + offset,
				c.From.StartTime
			);
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			var fadeOutDuration = 300;
			var fadeOutEasing = Easing.In;

			if ( state is ArmedState.Hit or ArmedState.Miss ) {
				piece.LightUp();

				var nextSwapTile = 
					HitObject.ToNextIs( x => HitObject.OrbitalState.IsNthPivot( x.TargetOrbitalIndex ) ) // if we are moving the same orbital as lies on this tile, wait till its done
					? HitObject.Next
					: HitObject.AllNext.FirstOrDefault( x => x.OrbitalState.ActiveIndex != HitObject.OrbitalState.ActiveIndex );

				using ( BeginAbsoluteSequence( nextSwapTile?.StartTime ?? LatestTransformEndTime ) ) {
					piece.ScaleTo( 0, fadeOutDuration, fadeOutEasing );
				}

				LifetimeEnd = piece.LatestTransformEndTime;
			}
		}

		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, piece.PositionScale );
		}

		protected override void Update () {
			base.Update();

			piece.FromPosition.Value = HitObject.Previous?.Position;
			piece.ToPosition.Value = HitObject.Next?.Position;
			piece.AroundPosition.Value = HitObject.Position;
		}
	}
}
