using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class DrawableHitokori : Container, IHasTilePosition {
		public List<Orbital> Orbitals = new List<Orbital>();
		private int OrbitalIndex;
		public Orbital LastOrbital { get; private set; }

		private Orbital NextOrbital {
			get {
				LastOrbital = Orbitals[ OrbitalIndex = ( OrbitalIndex + 1 ) % Orbitals.Count ];

				FirstFreeOrbital.MakeImportant();
				LastOrbital.RevokeImportant();

				return LastOrbital;
			}
		}
		private Orbital FirstFreeOrbital => Orbitals[ ( OrbitalIndex + 1 ) % Orbitals.Count ];
		private IEnumerable<Orbital> FreeOrbitals => Orbitals.Where( x => x != LastOrbital );
		bool Triplets;

		public void AddTriplet () {
			if ( Triplets ) return;

			var triplet = new TheUnwantedChild( this, Radius ).Center();
			Orbitals.Add( triplet );
			AddInternal( triplet );

			Triplets = true;
		}


		public Radius Radius;

		public Orbital Hi { get; set; }
		public Orbital Kori { get; set; }

		public DrawableHitokori () {
			TilePosition = InitialPosition;

			InternalChildren = new Drawable[] {
				Radius = new Radius { Depth = 1 }.Center(),
				Hi = new Hi( this, Radius ).Center(),
				Kori = new Kori( this, Radius ).Center()
			};

			Orbitals.Add( Hi );
			Orbitals.Add( Kori );
		}

		public double EndTime { get; private set; }
		public void Swap ( TilePoint hit ) {
			Snap();
			Swap();

			TilePosition = hit.TilePosition;
			RotateTo( hit.OutAngle, hit.HitTime, hit.HitTime + hit.Duration );
			AnimateDistance( duration: hit.StartTime + hit.Duration - Clock.CurrentTime, distance: DrawableTapTile.SPACING * ( hit.Next?.Distance ?? 1 ), easing: Easing.None );
		}

		public void Swap () {
			if ( LastOrbital is null ) {
				NextOrbital.Hold();
				FreeOrbitals.ForEach( x => x.Release() );
			} else {
				LastOrbital.Release();
				NextOrbital.Hold();
			}

			if ( Triplets ) {
				RotateTo( previousTargetRotation - ( Math.PI - TripletAngle ) );
			} else {
				RotateTo( previousTargetRotation - Math.PI );
			}
		}

		/// <summary>
		/// When contracted, expands radius using the given <see cref="Orbital"/> ( the last active one by default )
		/// </summary>
		public void AnimateDistance ( double duration = 500, double distance = DrawableTapTile.SPACING, Easing easing = Easing.InOutCubic ) {
			Radius.AnimateDistance( distance, Math.Max( duration, 0 ), easing );
		}

		public void Expand ( double duration = 500, Easing easing = Easing.InOutCubic )
			=> AnimateDistance( duration, easing: easing );

		public void Contract ( double duration = 500, Easing easing = Easing.InOutCubic ) {
			Radius.AnimateDistance( 0, duration, easing );
		}

		private double previousTargetRotation;
		public void VelocityConsistentRotateTo ( double target, double startTime, double endTime ) {
			EndTime = endTime;

			double duration = endTime - startTime;
			double actualDuration = endTime - Clock.CurrentTime;

			if ( Triplets ) target = ConvertToTripletAngle( target );

			if ( duration == 0 || actualDuration == 0 ) {
				RotateTo( target );
				Orbitals.ForEach( x => x.Velocity = 0 );
				return;
			}

			double velocity = ( target - previousTargetRotation ) / duration;

			double startAngle = previousTargetRotation;

			double leadupTime = actualDuration - duration;
			double angleOffset = leadupTime * velocity;
			RotateToWithInterpolation( startAngle - angleOffset );

			Orbitals.ForEach( x => x.Velocity = velocity );

			previousTargetRotation = target;
		}

		public void AngleConsistentRotateTo ( double target, double startTime, double endTime ) {
			EndTime = endTime;

			double duration = endTime - startTime;
			double actualDuration = endTime - Clock.CurrentTime;

			if ( Triplets ) target = ConvertToTripletAngle( target );

			if ( duration == 0 || actualDuration == 0 ) {
				RotateTo( target );
				Orbitals.ForEach( x => x.Velocity = 0 );
				return;
			}

			double angleOffset = LastOrbital.Angle - previousTargetRotation;
			double startAngle = previousTargetRotation - angleOffset;

			RotateToWithInterpolation( startAngle );
			double velocity = ( target - startAngle ) / actualDuration;

			Orbitals.ForEach( x => x.Velocity = velocity );

			previousTargetRotation = target;
		}

		private double ConvertToTripletAngle ( double angle ) {
			return angle - TripletAngle;
		}

		/// <summary>
		/// Rotates the Hitokori from interpolated start angle to <paramref name="target"/>
		/// </summary>
		/// <param name="tileIndex">TileIndex is used to correct rotation for triplets</param>
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

			if ( Triplets ) { // if yall want to find a generic formula, go for it
				Orbitals.ForEach( x => x.Angle = target );
				FirstFreeOrbital.Angle += TripletAngle;
			} else {
				Orbitals.ForEach( x => x.Angle = target );
			}
		}

		public void RotateToWithInterpolation ( double target ) {
			previousTargetRotation = target;

			if ( Triplets ) { // if yall want to find a generic formula, go for it
				foreach ( var x in Orbitals ) {
					if ( x == FirstFreeOrbital ) {
						x.RotateTo( target + TripletAngle );
					} else {
						x.RotateTo( target );
					}
				}
			} else {
				Orbitals.ForEach( x => x.RotateTo( target ) );
			}
		}

		const double TripletAngle = Math.PI / 3;

		/// <summary>
		/// Instantly rotates to the target rotation
		/// </summary>
		public void Snap () {
			RotateToWithInterpolation( previousTargetRotation );
		}

		public Vector2 TilePosition { get; private set; }
		public static readonly Vector2 InitialPosition = Vector2.Zero;

		public Vector2 HiOffset => Hi.Position;
		public Vector2 KoriOffset => Kori.Position;


		Bindable<MissCorrectionMode> CorrectionMode = new Bindable<MissCorrectionMode>();
		[BackgroundDependencyLoader]
		private void load ( HitokoriSettingsManager config ) {
			CorrectionMode = config.GetBindable<MissCorrectionMode>( HitokoriSetting.MissCorrectionMode );
		}

		public double StableAngle {
			get {
				if ( Triplets ) {
					double offset = OrbitalIndex switch {
						/* Hi   */ 0 => 0,
						/* Kori */ 1 => Math.PI * 4 / 3,
						           _ => Math.PI
					}; // dont ask why these values i Dont kn o w

					return ( Hi.Angle - offset ).ToDegrees();
				} else {
					return ( Hi.Angle + ( ( LastOrbital == Hi ) ? Math.PI : 0 ) ).ToDegrees();
				}
			}
		}
	}
}
