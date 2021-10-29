﻿using osu.Framework.Bindables;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Hitokori.Edit.Blueprints;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
using osu.Game.Rulesets.Hitokori.Orbitals;
using osu.Game.Rulesets.Objects.Types;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects {
	/// <summary>
	/// A point where an orbital may rest.
	/// </summary>
	public abstract class TilePoint : HitokoriHitObject, IHasPosition {
		public TilePoint () {
			ConstrainablePosition = new ConstrainableProperty<Vector2d>( 
				() => ConstrainablePosition!.Value = fromPrevious!.GetEndPosition(),
				onConstraintChanged,
				() => BindablePosition.Value = Position!
			);

			BindablePosition.ValueChanged += v => ConstrainablePosition.Value = v.NewValue;
		}

		private void onConstraintChanged ( bool isConstrained ) {
			Invalidate();
		}

		/// <summary>
		/// Force this and any subsequent <see cref="TilePoint"/>s to recalcuate their properties such as <see cref="Position"/>.
		/// </summary>
		public void Invalidate () {
			if ( !IsInvalidationPossible ) return;

			InvalidateProperties();
			ToNext?.Invalidate();
		}

		/// <summary>
		/// Invalidates all properties of this <see cref="TilePoint"/>.
		/// </summary>
		protected virtual void InvalidateProperties () {
			ConstrainablePosition.Invalidate();
			isOrbitalStateComputed = false;
		}

		/// <summary>
		/// Whether any properties are already computed.
		/// </summary>
		protected virtual bool IsInvalidationPossible => (!ConstrainablePosition.IsConstrained && ConstrainablePosition.IsComputed) || (!IsOrbitalStateConstrained && isOrbitalStateComputed);

		public readonly Bindable<Vector2d> BindablePosition = new Bindable<Vector2d>();
		public readonly ConstrainableProperty<Vector2d> ConstrainablePosition;
		public Vector2d Position => ConstrainablePosition;
		public Vector2d ConstrainPosition { set => ConstrainablePosition.Constrain( value ); }

		private OrbitalState orbitalState;
		/// <summary>
		/// Is this <see cref="TilePoint"/>'s <see cref="OrbitalState"/> fixed (does not depend on the <see cref="TilePointConnector.GetEndPosition"/>).
		/// </summary>
		public bool IsOrbitalStateConstrained { get; private set; } = false;
		private bool isOrbitalStateComputed = false;
		/// <summary>
		/// State of orbitals. Setting this property will constrain it.
		/// </summary>
		public OrbitalState OrbitalState {
			get {
				if ( !IsOrbitalStateConstrained && !isOrbitalStateComputed ) {
					orbitalState = ModifyOrbitalState( fromPrevious!.GetEndState() );
					isOrbitalStateComputed = true;
				}
				return orbitalState;
			}
			set {
				IsOrbitalStateConstrained = true;
				orbitalState = value;
			}
		}

		/// <summary>
		/// Modify and return an orbital state as appropriate for this <see cref="TilePoint"/>'s interaction.
		/// </summary>
		public virtual OrbitalState ModifyOrbitalState ( OrbitalState original ) => original;

		private TilePointConnector? fromPrevious;
		/// <summary>
		/// A connection coming from a previous <see cref="TilePoint"/>.
		/// </summary>
		public TilePointConnector? FromPrevious {
			get => fromPrevious;
			set {
				if ( fromPrevious == value ) return;
				var old = fromPrevious;
				fromPrevious = value;

				if ( fromPrevious is not null ) {
					fromPrevious.To = this;
				}
				else {
					old!.To = null;
					old.From = null;
				}

				Invalidate();
			}
		}

		private TilePointConnector? toNext;
		/// <summary>
		/// A connection coming to the next <see cref="TilePoint"/>.
		/// </summary>
		public TilePointConnector? ToNext {
			get => toNext;
			set {
				if ( toNext == value ) return;
				var old = toNext;
				toNext = value;

				if ( toNext is not null ) {
					toNext.From = this;
				}
				else {
					old!.From = null;
					old.To = null;
				}

				Invalidate();
			}
		}

		public TilePoint? Next => ToNext?.To;
		public TilePoint? Previous => FromPrevious?.From;

		public TilePoint First {
			get {
				TilePoint tp = this;
				while ( tp.Previous is not null ) {
					tp = tp.Previous;
				}
				return tp;
			}
		}
		public TilePoint Last {
			get {
				TilePoint tp = this;
				while ( tp.Next is not null ) {
					tp = tp.Next;
				}
				return tp;
			}
		}

		public IEnumerable<TilePoint> AllNext {
			get {
				TilePoint? tp = Next;
				while ( tp is not null ) {
					yield return tp;
					tp = tp.Next;
				}
			}
		}

		public IEnumerable<TilePoint> AllPrevious {
			get {
				TilePoint? tp = Previous;
				while ( tp is not null ) {
					yield return tp;
					tp = tp.Previous;
				}
			}
		}

		public IEnumerable<TilePoint> AllInChain
			=> First.AllNext;

		/// <summary>
		/// To which chain this <see cref="TilePoint"/> belongs to.
		/// Setting this value will only have impact during the <see cref="Beatmaps.HitokoriBeatmapProcessor"/> linking phase.
		/// </summary>
		public int ChainID;

		public bool ToNextIs ( Predicate<TilePointConnector> predicate )
			=> ToNext is null
			? false
			: predicate( ToNext );

		public bool NextIs ( Predicate<TilePoint> predicate )
			=> Next is null
			? false
			: predicate( Next );

		public bool FromPreviousIs ( Predicate<TilePointConnector> predicate )
			=> FromPrevious is null
			? false
			: predicate( FromPrevious );

		public bool PreviousIs ( Predicate<TilePoint> predicate )
			=> Previous is null
			? false
			: predicate( Previous );

		public double LifetimeOffset 
			=> Math.Max( 2000, FromPrevious is null ? 0 : FromPrevious.Duration );

		Vector2 IHasPosition.Position => (Vector2)Position;
		float IHasXPosition.X => (float)Position.X;
		float IHasYPosition.Y => (float)Position.Y;

		public override HitObjectSelectionBlueprint? CreateSelectionBlueprint ()
			=> new TilePointSelectionBlueprint( this );
	}
}
