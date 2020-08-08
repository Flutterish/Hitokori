using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class DrawableHitokori : Container, IHasTilePosition { // TODO prep for n orbitals
		public Radius Radius;

		private Orbital active;
		public Orbital Active { 
			get => active; 
			private set {
				if ( value != null && Active != null ) {
					LastActive = Active;
				}
				active = value;
			}
		}
		public Orbital Inactive => ( Active == Hi ) ? Kori : Hi;
		public Orbital LastActive { get; private set; }
		public Orbital Hi;
		public Orbital Kori;

		public DrawableHitokori () {
			TilePosition = InitialPosition;

			InternalChildren = new Drawable[] {
				Radius = new Radius { Depth = 1 }.Center(),
				Hi = new Hi( this ).Center(),
				Kori = new Kori( this ).Center()
			};
		}

		public double EndTime { get; private set; }
		public void Swap ( TilePoint hit ) {
			Snap();
			Swap();

			TilePosition = hit.TilePosition;
			RotateTo( hit.OutAngle, hit.HitTime, hit.HitTime + hit.Duration );
			AnimateDistance( Active, duration: hit.StartTime + hit.Duration - Clock.CurrentTime, distance: DrawableTapTile.SPACING * ( hit.Next?.Distance ?? 1 ), easing: Easing.None );
		}

		public void Swap () {
			if ( Active is null ) {
				AnimateDistance( Hi );
			} else {
				Inactive.Release( Active.Distance );
				Active.Hold();

				Active = Inactive;
			}

			RotateTo( previousTargetRotation - Math.PI );
		}

		/// <summary>
		/// When contracted, expands radius using the given <see cref="Orbital"/> ( the last active one by default )
		/// </summary>
		public void AnimateDistance ( Orbital orbital = null, double duration = 500, double distance = DrawableTapTile.SPACING, Easing easing = Easing.InOutCubic ) {
			if ( orbital is null ) orbital = LastActive;

			Active = orbital;
			Active.AnimateDistance( distance, Math.Max( duration, 0 ), easing );

			Radius.AnimateDistance( distance, Math.Max( duration, 0 ), easing );
		}

		public void Expand ( Orbital orbital = null, double duration = 500, Easing easing = Easing.InOutCubic )
			=> AnimateDistance( orbital, duration, easing: easing );

		public void Contract ( double duration = 500, Easing easing = Easing.InOutCubic ) {
			if ( Active is null ) return;

			ChangeChildDepth( Active, 1 );
			ChangeChildDepth( Inactive, -1 );

			Active.MoveTo( Vector2.Zero, duration, easing );
			Active.AnimateDistance( 0, duration, easing );
			Radius.AnimateDistance( 0, duration, easing );

			Active = null;
		}

		private double previousTargetRotation;
		public void VelocityConsistentRotateTo ( double target, double startTime, double endTime ) {
			EndTime = endTime;

			double duration = endTime - startTime;
			double actualDuration = endTime - Clock.CurrentTime;

			if ( duration == 0 || actualDuration == 0 ) {
				RotateTo( target );
				Hi.Velocity = 0;
				Kori.Velocity = 0;
				return;
			}

			double velocity = ( target - previousTargetRotation ) / duration;

			double startAngle = previousTargetRotation;

			double leadupTime = actualDuration - duration;
			double angleOffset = leadupTime * velocity;
			RotateToWithInterpolation( startAngle - angleOffset );

			Hi.Velocity = velocity;
			Kori.Velocity = velocity;

			previousTargetRotation = target;
		}

		public void AngleConsistentRotateTo ( double target, double startTime, double endTime ) {
			EndTime = endTime;

			double duration = endTime - startTime;
			double actualDuration = endTime - Clock.CurrentTime;

			if ( duration == 0 || actualDuration == 0 ) {
				RotateTo( target );
				Hi.Velocity = 0;
				Kori.Velocity = 0;
				return;
			}

			double angleOffset = Hi.Angle - previousTargetRotation;
			double startAngle = previousTargetRotation - angleOffset;

			RotateToWithInterpolation( startAngle );
			double velocity = ( target - startAngle ) / actualDuration;

			Hi.Velocity = velocity;
			Kori.Velocity = velocity;

			previousTargetRotation = target;
		}

		/// <summary>
		/// Rotates the Hitokori from interpolated start angle to <paramref name="target"/>
		/// </summary>
		/// <param name="target">The target in radians</param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		public void RotateTo ( double target, double startTime, double endTime ) {
			switch ( CorrectionMode.Value ) {
				case MissCorrectionMode.Angle:
					AngleConsistentRotateTo( target, startTime, endTime );
					break;

				case MissCorrectionMode.Velocity:
					VelocityConsistentRotateTo( target, startTime, endTime );
					break;

				default:
					throw new InvalidOperationException( "How the fuck" );
			}
		}

		/// <summary>
		/// Rotates to the given angle instantly. Use this only for snapping
		/// </summary>
		public void RotateTo ( double target ) {
			previousTargetRotation = target;

			Hi.Angle = target;
			Kori.Angle = target;
		}

		public void RotateToWithInterpolation ( double target ) {
			previousTargetRotation = target;

			Hi.RotateTo( target );
			Kori.RotateTo( target );
		}

		/// <summary>
		/// Instantly rotates to the target rotation
		/// </summary>
		public void Snap () {
			RotateToWithInterpolation( previousTargetRotation );
		}

		public Vector2 TilePosition { get; private set; }
		public static readonly Vector2 InitialPosition = Vector2.Zero;

		public Vector2 ActiveOffset
			=> ( Active is null ) ? Vector2.Zero : Active.Position;
		public Vector2 ActivePosition
			=> TilePosition + ActiveOffset;
		public Vector2 HiOffset => Hi.Position;
		public Vector2 KoriOffset => Kori.Position;


		Bindable<MissCorrectionMode> CorrectionMode = new Bindable<MissCorrectionMode>();
		[BackgroundDependencyLoader]
		private void load ( HitokoriSettingsManager config ) {
			CorrectionMode = config.GetBindable<MissCorrectionMode>( HitokoriSetting.MissCorrectionMode );
		}
	}
}
