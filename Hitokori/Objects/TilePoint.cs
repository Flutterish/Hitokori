﻿using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables;
using osu.Game.Rulesets.Hitokori.Scoring;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Objects { // TODO ability to recalculate everything recursively with children ( for animated/rotating tiles )
	public class TilePoint : /*Nested*/HitokoriHitObject, IHasTilePosition {
		public bool useTripletAngles;
		public double Offset => ( useTripletAngles ? ( Math.PI / 3 ) : 0 );

		/// <summary>
		/// <see cref="TilePoint"/> around which this tile rotates
		/// </summary>
		public TilePoint Parent;
		public TilePoint Previous;
		public TilePoint Next;

		public bool IsFirst => Previous is null;
		public bool IsLast => Next is null;

		public double SpeedModifier = 1;
		public bool IsClockwise = true;
		public double Direction => IsClockwise ? 1 : -1;
		public bool ChangedDirection {
			get => Previous.IsClockwise != IsClockwise;
			set => IsClockwise = ( ChangedDirection == value ) ? IsClockwise : !IsClockwise;
		}

		public bool AllowDynamicDistance = true;
		private double distance = 1;
		private double maxDynamicDistance = 2;
		public double Distance {
			get {
				if ( AllowDynamicDistance && !IsFirst ) {
					return Math.Min( Previous.NextRequestedDistance, maxDynamicDistance );
				}
				else return distance;
			}
			set {
				distance = value;
				AllowDynamicDistance = false;
			}
		}
		/// <summary>
		/// The next tile should be this distance, if possible.
		/// </summary>
		public double NextRequestedDistance {
			get {
				if ( arclength / distance > MAX_ANGLE ) {
					return BEAT_STRETCH / MAX_ANGLE * Beats * SpeedModifier; // TODO request contant speed instead of angle
				}
				else return Next.distance;
			}
		}

		public const double MAX_ANGLE = Math.PI * 1.75;
		/// <summary>
		/// Arclength per beat
		/// </summary>
		private double BEAT_STRETCH => useTripletAngles ? ( Math.PI * 2 / 3 ) : Math.PI * 2 / 3;

		public double BPMS;
		public double HitTime {
			get => StartTime;
			set => StartTime = value;
		}
		public double TimeToRotate ( double radians )
			=> radians / BPMS / SpeedModifier / BEAT_STRETCH * distance;
		public double Duration => Math.Max( IsLast ? TimeToRotate( MAX_ANGLE ) : Next.HitTime - HitTime, 0 );
		public double TimeToStart ( double currentTime ) => HitTime - currentTime;
		public double Beats => BPMS * Duration;

		private double arclength => Beats * SpeedModifier * BEAT_STRETCH;
		public double AngleOffset => Direction * Math.Clamp( arclength / Distance, 0, MAX_ANGLE );
		public double AngleFromStraight => AngleOffset - Math.PI;

		protected bool isOutAngleCached = false;
		protected double cachedOutAngle;
		public double OutAngle {
			get {
				if ( !isOutAngleCached ) {
					if ( Parent == Previous )
						cachedOutAngle = InAngle + AngleFromStraight + Offset;
					else if ( Parent == Previous.Parent )
						cachedOutAngle = InAngle + AngleOffset;
					else
						throw new InvalidOperationException( "No suitable rotation origin" );
					isOutAngleCached = true;
				}

				return cachedOutAngle;
			}
		}
		public double FlippedOutAngle {
			get {
				if ( Previous is null ) return 0;

				if ( Parent == Previous )
					return InAngle - AngleFromStraight + Offset;
				else if ( Parent == Previous.Parent )
					return InAngle - AngleOffset;
				else
					throw new InvalidOperationException( "No suitable rotation origin" );
			}
		}
		public double InAngle => Previous.OutAngle;

		public HitokoriAngleHitWindows AngleHitWindows;
		public HitokoriHitWindows TimeHitWindows;

		public double TimeAtOffset ( double offset )
			=> HitTime + offset;
		public double TimeOffsetAt ( double time ) {
			return time - HitTime;
		}
		public double AngleOffsetAt ( double time ) {
			return Previous.Speed.ToDegrees() * TimeOffsetAt( time );
		}
		public bool CanBeHitAfter ( double time ) {
			return TimeHitWindows.CanBeHit( TimeOffsetAt( time ) );
		}
		public HitResult ResultAt ( double time ) {
			var result = TimeHitWindows.ResultFor( TimeOffsetAt( time ) );
			if ( result == HitResult.None && TimeOffsetAt( time ) > -500 ) return HitResult.Miss;
			if ( result is HitResult.Perfect or HitResult.Miss or HitResult.None ) return result;
			if ( TimeOffsetAt( time ) > 0 )
				return HitResult.Great;
			else
				return HitResult.Ok;
		}

		protected bool isPositionCached = false;
		protected Vector2 cachedPosition;
		public Vector2 NormalizedTilePosition {
			get {
				if ( !isPositionCached ) {
					if ( Parent == Previous )
						cachedPosition = Parent.NormalizedTilePosition + new Vector2( (float)Math.Cos( InAngle ), (float)Math.Sin( InAngle ) ) * (float)Distance;
					else if ( Parent == Previous.Parent )
						cachedPosition = Parent.NormalizedTilePosition + new Vector2( (float)Math.Cos( InAngle + Math.PI - Offset ), (float)Math.Sin( InAngle + Math.PI - Offset ) ) * (float)Distance; // because we are not going straight on
					else
						throw new InvalidOperationException( "No suitable rotation origin" );

					isPositionCached = true;
				}

				return cachedPosition;
			}
		}
		public Vector2 FlippedNormalizedTilePosition {
			get {
				if ( Parent == Previous )
					return Parent.NormalizedTilePosition + new Vector2( (float)Math.Cos( Previous.FlippedOutAngle ), (float)Math.Sin( Previous.FlippedOutAngle ) ) * (float)Distance;
				else if ( Parent == Previous.Parent )
					return Parent.NormalizedTilePosition + new Vector2( (float)Math.Cos( Previous.FlippedOutAngle + Math.PI - Offset ), (float)Math.Sin( Previous.FlippedOutAngle + Math.PI - Offset ) ) * (float)Distance; // because we are not going straight on
				else
					throw new InvalidOperationException( "No suitable rotation origin" );
			}
		}
		public Vector2 TilePosition
			=> NormalizedTilePosition * HitokoriTile.SPACING;

		public IEnumerable<TilePoint> GetWholeChain () {
			List<TilePoint> chain = new List<TilePoint>();
			var el = this;

			while ( el.Previous != null ) el = el.Previous;
			chain.Add( el );
			while ( el.Next != null ) {
				el = el.Next;
				chain.Add( el );
			}

			return chain.Skip( 1 ); // first tile point
		}
		public IEnumerable<TilePoint> GetPrevious ( int count ) {
			List<TilePoint> chain = new List<TilePoint>();
			var el = this;
			while ( el.Previous != null ) {
				el = el.Previous;
				chain.Add( el );
			}
			return chain;
		}

		public void RefreshChain () {
			foreach ( var i in GetWholeChain() ) {
				i.Refresh();
			}
		}

		public void Refresh () {
			isOutAngleCached = false;
			isPositionCached = false;
		}

		/// <summary>
		/// ( Unsigned ) Speed in radians per millisecond
		/// </summary>
		public double Speed => Math.Abs( Velocity );
		/// <summary>
		/// ( Signed ) Speed in radians per millisecond
		/// </summary>
		public double Velocity => AngleOffset * Distance / Duration;
		/// <summary>
		/// Is this tile slower than the previous one?
		/// </summary>
		public bool IsSlower => SpeedDifferencePercent <= -0.01;
		/// <summary>
		/// Is this tile faster than the previous one?
		/// </summary>
		public bool IsFaster => SpeedDifferencePercent >= 0.01;
		public double SpeedDifferece
			=> Speed - Previous.Speed;
		public double SpeedDifferencePercent
			=> ( Speed / Previous.Speed ) - 1;
		public double VelocityDifference
			=> Velocity - Previous.Velocity;
		public bool IsDifferentSpeed => IsFaster || IsSlower;

		public Colour4 Color // TODO make the intensity depened on SpeedDifferencePercent
			=> IsFaster ? Colour4.Orange
			: IsSlower ? Colour4.DodgerBlue
			: Colour4.White;

		public TickSize Size
			=> IsDifferentSpeed ? TickSize.Big : TickSize.Regular;

		/// <summary>
		/// Joins the two <see cref="TilePoint"/>s and returns the latter
		/// </summary>
		public TilePoint Then ( TilePoint next ) {
			Next = next;
			next.Previous = this;

			return next;
		}

		public override DrawableHitokoriHitObject AsDrawable ()
			=> null;

		public override Judgement CreateJudgement ()
			=> new HitokoriJudgement();
	}
}
