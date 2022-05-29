﻿namespace osu.Game.Rulesets.Hitokori.Orbitals {
	/// <summary>
	/// Represents the state of multiple orbitals at a given time.
	/// </summary>
	public struct OrbitalState {
		/// <param name="sortByAngle">Whether to sort the orbitals by angle with <see cref="Vector2.Zero"/> as centre.</param>
		public OrbitalState ( IEnumerable<Vector2d> positions, bool sortByAngle = true ) {
			InitialPositions = sortByAngle ? positions.OrderByDescending( x => Vector2d.Zero.AngleTo( x ) ).ToArray() : positions.ToArray();

			if ( InitialPositions.Count < 1 ) throw new InvalidOperationException( $"{nameof(OrbitalState)} needs at least 1 orbital but was constructed with {InitialPositions.Count}" );

			PivotPosition = Vector2d.Zero;
			ActiveIndex = 0;
			TotalRotation = 0;
			Scale = 1;
			Z = 0;
			StackingOffset = Vector2d.Zero;

			var centre = InitialPositions.Aggregate( Vector2d.Zero, ( a, b ) => a + b ) / InitialPositions.Count;
			var radius = InitialPositions.Max( x => ( x - centre ).Length );
			OriginalEnclosingCircle = (centre, radius);
		}

		public double Scale { get; private set; }
		public double Z { get; private set; }
		public IReadOnlyList<Vector2d> InitialPositions { get; private set; }
		private Vector2d internalPivotPosition => InitialPositions[ ActiveIndex.Mod( InitialPositions.Count ) ];
		public Vector2d PivotPosition { get; private set; }
		/// <summary>
		/// Offset generally used to create stacks of tiles.
		/// </summary>
		public Vector2d StackingOffset { get; private set; }

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
		/// Whether the original orbital at the given index is the pivot.
		/// </summary>
		public bool IsNthOriginalPivot ( int index )
			=> ( index - ActiveIndex ).Mod( OrbitalCount ) == 0;

		/// <summary>
		/// Whether the orbital at a given offset from current pivot is the pivot.
		/// </summary>
		public bool IsNthPivot ( int index )
			=> index.Mod( OrbitalCount ) == 0;

		/// <summary>
		/// Offset from the current pivot of Nth orbital from the pivot.
		/// </summary>
		public Vector2d OffsetOfNth ( int index ) => (InitialPositions[ (index + ActiveIndex).Mod( InitialPositions.Count ) ] - internalPivotPosition).Rotate( (float)TotalRotation ) * Scale
			+ StackingOffsetOfNth( index );
		/// <summary>
		///  Offset from the current pivot of Nth orbital.
		/// </summary>
		public Vector2d OffsetOfNthOriginal ( int index ) => (InitialPositions[ index.Mod( InitialPositions.Count ) ] - internalPivotPosition).Rotate( (float)TotalRotation ) * Scale
			+ StackingOffsetOfNthOriginal( index );

		public Vector2d StackingOffsetOfNth ( int index )
			=> IsNthPivot( index ) ? Vector2d.Zero : StackingOffset;
		public Vector2d StackingOffsetOfNthOriginal ( int index )
			=> IsNthOriginalPivot( index ) ? Vector2d.Zero : StackingOffset;

		/// <summary>
		/// Position of the Nth orbital from current pivot.
		/// </summary>
		public Vector2d PositionOfNth ( int index ) => PivotPosition + OffsetOfNth( index );
		/// <summary>
		/// Position of the Nth orbital.
		/// </summary>
		public Vector2d PositionOfNthOriginal ( int index ) => PivotPosition + OffsetOfNthOriginal( index );

		/// <summary>
		/// Offset from the current pivot of Nth orbital from the pivot without applying scaling.
		/// </summary>
		public Vector2d UnscaledOffsetOfNth ( int index ) => ( InitialPositions[ ( index + ActiveIndex ).Mod( InitialPositions.Count ) ] - internalPivotPosition ).Rotate( (float)TotalRotation )
			+ StackingOffsetOfNth( index );
		/// <summary>
		///  Offset from the current pivot of Nth orbital without applying scaling.
		/// </summary>
		public Vector2d UnscaledOffsetOfNthOriginal ( int index ) => ( InitialPositions[ index.Mod( InitialPositions.Count ) ] - internalPivotPosition ).Rotate( (float)TotalRotation )
			+ StackingOffsetOfNthOriginal( index );

		/// <summary>
		/// Position of the Nth orbital from current pivot without applying scaling.
		/// </summary>
		public Vector2d UnscaledPositionOfNth ( int index ) => PivotPosition + UnscaledOffsetOfNth( index );
		/// <summary>
		/// Position of the Nth orbital without applying scaling.
		/// </summary>
		public Vector2d UnscaledPositionOfNthOriginal ( int index ) => PivotPosition + UnscaledOffsetOfNthOriginal( index );

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
		/// Creates a new instance where the pivot changed by <paramref name="index"/> swaps. Resets <see cref="StackingOffset"/>.
		/// </summary>
		public OrbitalState PivotNth ( int index ) => this with {
			PivotPosition = IsNthPivot( index ) ? PivotPosition : (PivotPosition + StackingOffset),
			ActiveIndex = ActiveIndex + index,
			StackingOffset = Vector2d.Zero
		};

		/// <summary>
		/// Creates a new instance where the pivot changed by <paramref name="index"/> swaps. Resets <see cref="StackingOffset"/>.
		/// </summary>
		public OrbitalState PivotNth ( int index, Vector2d newPosition ) => this with {
			ActiveIndex = ActiveIndex + index,
			PivotPosition = newPosition,
			StackingOffset = Vector2d.Zero
		};

		public OrbitalState WithScale ( double scale ) => this with {
			Scale = scale
		};

		/// <summary>
		/// Creates a new instance where every oribital except the pivot is offset by the given amout. Gererally used to create stacks of tiles.
		/// Note that when swapping tiles, this creates a visual jump.
		/// </summary>
		public OrbitalState WithStackingOffset ( Vector2d offset ) => this with {
			StackingOffset = offset
		};

		/// <summary>
		/// Creates a new instance at a different elevation. This moves and scales the orbitals up.
		/// </summary>
		public OrbitalState WithZ ( double z ) => this with {
			Z = z
		};
	}
}
