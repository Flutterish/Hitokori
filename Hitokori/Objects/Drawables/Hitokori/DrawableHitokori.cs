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
		public Orbital HoldingOrbital { get; private set; }

		private Orbital SwitchToNextOrbital () {
			HoldingOrbital = Orbitals[ OrbitalIndex = ( OrbitalIndex + 1 ) % Orbitals.Count ];

			NextOrbital.MakeImportant();
			HoldingOrbital.RevokeImportant();

			return HoldingOrbital;
		}
		private Orbital NextOrbital => Orbitals[ ( OrbitalIndex + 1 ) % Orbitals.Count ];
		private IEnumerable<Orbital> FreeOrbitals => Orbitals.Where( x => x != HoldingOrbital );
		bool Triplets;

		public void AddTriplet () {
			if ( Triplets ) return;

			var triplet = new TheUnwantedChild( this, Radius ).Center();
			Orbitals.Add( triplet );
			AddInternal( triplet );

			Triplets = true;
		}


		public Radius Radius;

		public Orbital Hi { get; private set; }
		public Orbital Kori { get; private set; }

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
		private double lastDistance = DrawableTapTile.SPACING;
		public void Swap ( TilePoint hit ) {
			NextOrbital.AnimateFromHere( 140, Easing.InBack );
			Snap();
			Swap();

			TilePosition = hit.TilePosition;

			RotateTo( hit.OutAngle, hit.HitTime, hit.HitTime + hit.Duration );
			lastDistance = DrawableTapTile.SPACING * ( hit.Next?.Distance ?? 1 );
			AnimateDistance( duration: hit.StartTime + hit.Duration - Clock.CurrentTime, distance: lastDistance, easing: Easing.None );
		}

		public void Swap () {
			if ( HoldingOrbital is null ) {
				SwitchToNextOrbital().Hold();
				FreeOrbitals.ForEach( x => x.Release() );
			}
			else {
				HoldingOrbital.Release();
				SwitchToNextOrbital().Hold();
			}

			if ( Triplets ) {
				RotateTo( previousTargetRotation - ( Math.PI - TripletAngle ) );
			}
			else {
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
			=> AnimateDistance( duration, lastDistance, easing );

		public void Contract ( double duration = 500, Easing easing = Easing.InOutCubic ) {
			Radius.AnimateDistance( 0, duration, easing );
		}

		private double previousTargetRotation;

		public void NoOffsetRotateTo ( double target, double startTime, double endTime ) {
			EndTime = endTime;
			double actualDuration = endTime - Clock.CurrentTime;
			if ( Triplets ) target = ConvertToTripletAngle( target );
			if ( actualDuration < 0 ) {
				actualDuration = endTime - startTime;
			}
			if ( actualDuration <= 0 ) {
				RotateTo( target );
				Orbitals.ForEach( x => x.Velocity = 0 );
				return;
			}
			double velocity = ( target - previousTargetRotation ) / actualDuration;
			RotateToWithInterpolation( previousTargetRotation );
			Orbitals.ForEach( x => x.Velocity = velocity );
			Velocity = velocity;
			previousTargetRotation = target;
		}

		private double ConvertToTripletAngle ( double angle ) {
			return angle - TripletAngle;
		}

		/// <summary>
		/// Rotates the Hitokori from interpolated start angle to <paramref name="target"/>
		/// </summary>
		/// <param name="target">The target in radians</param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		public void RotateTo ( double target, double startTime, double endTime ) {
			NoOffsetRotateTo( target, startTime, endTime );
		}

		/// <summary>
		/// Rotates to the given angle instantly. Use this only for snapping
		/// </summary>
		public void RotateTo ( double target ) {
			previousTargetRotation = target;

			if ( Triplets ) { // if yall want to find a generic formula, go for it
				Orbitals.ForEach( x => x.Angle = target );
				NextOrbital.Angle += TripletAngle;
			}
			else {
				Orbitals.ForEach( x => x.Angle = target );
			}
		}

		public void RotateToWithInterpolation ( double target ) {
			previousTargetRotation = target;

			if ( Triplets ) { // if yall want to find a generic formula, go for it
				foreach ( var x in Orbitals ) {
					if ( x == NextOrbital ) {
						x.RotateTo( target + TripletAngle );
					}
					else {
						x.RotateTo( target );
					}
				}
			}
			else {
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

		private double velocity;
		private double startTime;
		private double startAngle;
		private double Velocity {
			get => velocity;
			set {
				startAngle = StableAngle;
				startTime = Clock.CurrentTime;
				velocity = value;
			}
		}

		public double StableAngle {
			get => startAngle + ( Clock.CurrentTime - startTime ) * Velocity;
			private set {
				startTime = Clock.CurrentTime;
				startAngle = value;
			}
		}

		public void OnPress () {
			held?.OnRelease();

			NextOrbital.OnPress();
		}

		private Orbital held;
		public void OnHold () {
			held?.OnRelease();
			( held = NextOrbital ).OnHold();
		}

		public void OnRelease () {
			held?.OnRelease();
		}
	}
}
