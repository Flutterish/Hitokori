using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Orbitals {
	/// <summary>
	/// Represents the state of multiple orbitals at a given time.
	/// </summary>
	public struct OrbitalState {
		/// <param name="sortByAngle">Whether to sort the orbitals by angle with <see cref="Vector2.Zero"/> as centre.</param>
		public OrbitalState ( IEnumerable<Vector2d> positions, bool sortByAngle = true ) {
			InitialPositions = sortByAngle ? positions.OrderByDescending( x => Vector2d.Zero.AngleTo( x ) ).ToArray() : positions.ToArray();

			if ( InitialPositions.Count < 2 ) throw new InvalidOperationException( $"{nameof(OrbitalState)} needs at least 2 orbitals but was constructed with {InitialPositions.Count}" );

			PivotPosition = Vector2d.Zero;
			ActiveIndex = 0;
			TotalRotation = 0;
			Scale = 1;
			Offset = Vector2d.Zero;

			var centre = InitialPositions.Aggregate( Vector2d.Zero, ( a, b ) => a + b ) / InitialPositions.Count;
			var radius = InitialPositions.Max( x => ( x - centre ).Length );
			OriginalEnclosingCircle = (centre, radius);
		}

		public double Scale { get; private set; }
		public IReadOnlyList<Vector2d> InitialPositions { get; private set; }
		private Vector2d internalPivotPosition => InitialPositions[ ActiveIndex.Mod( InitialPositions.Count ) ];
		public Vector2d PivotPosition { get; private set; }
		/// <summary>
		/// Offset generally used to create stacks of tiles.
		/// </summary>
		public Vector2d Offset { get; private set; }

		/// <summary>
		/// Accumulated actual rotation.
		/// </summary>
		public double TotalRotation { get; private set; }
		/// <summary>
		/// Which oribtal index is the current pivot.
		/// </summary>
		public int ActiveIndex { get; private set; }
		public int OrbitalCount => InitialPositions.Count;

		/// <summary>
		/// Offset from the current pivot of Nth orbital from the pivot.
		/// </summary>
		public Vector2d OffsetOfNth ( int index ) => (InitialPositions[ (index + ActiveIndex).Mod( InitialPositions.Count ) ] - internalPivotPosition).Rotate( (float)TotalRotation ) * Scale
			+ ( index.Mod( OrbitalCount ) == 0 ? Vector2d.Zero : Offset );
		/// <summary>
		///  Offset from the current pivot of Nth orbital.
		/// </summary>
		public Vector2d OffsetOfNthOriginal ( int index ) => (InitialPositions[ index.Mod( InitialPositions.Count ) ] - internalPivotPosition).Rotate( (float)TotalRotation ) * Scale
			+ ( index.Mod( OrbitalCount ) == ActiveIndex.Mod( OrbitalCount ) ? Vector2d.Zero : Offset ) ;

		/// <summary>
		/// Position of the Nth orbital from current pivot.
		/// </summary>
		public Vector2d PositionOfNth ( int index ) => PivotPosition + OffsetOfNth( index );
		/// <summary>
		/// Position of the Nth orbital.
		/// </summary>
		public Vector2d PositionOfNthOriginal ( int index ) => PivotPosition + OffsetOfNthOriginal( index );

		/// <summary>
		/// A circle that encloses all the original orbitals. This might not be the smallest possible circle that achieves this.
		/// </summary>
		public (Vector2d centre, double radius) OriginalEnclosingCircle { get; private set; }

		/// <summary>
		/// A circle that encloses all the orbitals. This might not be the smallest possible circle that achieves this.
		/// </summary>
		public (Vector2d offsetFromPivot, double radius) EnclosingCircle => ( (OriginalEnclosingCircle.centre - internalPivotPosition).Rotate( TotalRotation ), OriginalEnclosingCircle.radius * Scale );

		/// <summary>
		/// Creates a new instance rotated by <paramref name="angle"/> radians clockwise.
		/// </summary>
		public OrbitalState RotatedBy ( double angle ) => this with {
			TotalRotation = TotalRotation + angle
		};
		
		/// <summary>
		/// Creates a new instance where the pivot changed by <paramref name="index"/> swaps. Resets <see cref="Offset"/>.
		/// </summary>
		public OrbitalState PivotNth ( int index ) => this with {
			ActiveIndex = ActiveIndex + index,
			Offset = Vector2d.Zero
		};

		/// <summary>
		/// Creates a new instance where the pivot changed by <paramref name="index"/> swaps. Resets <see cref="Offset"/>.
		/// </summary>
		public OrbitalState PivotNth ( int index, Vector2d newPosition ) => this with {
			ActiveIndex = ActiveIndex + index,
			PivotPosition = newPosition,
			Offset = Vector2d.Zero
		};

		public OrbitalState WithScale ( double scale ) => this with {
			Scale = scale
		};

		/// <summary>
		/// Creates a new instance where every oribital except the pivot is offset by the given amout. Gererally used to create stacks of tiles.
		/// </summary>
		public OrbitalState WithOffset ( Vector2d offset ) => this with {
			Offset = offset
		};
	}
}
