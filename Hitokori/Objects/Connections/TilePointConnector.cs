using osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors;
using osu.Game.Rulesets.Hitokori.Orbitals;
using osuTK;
using System;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Objects {
	/// <summary>
	/// A rotation, linear movement or whatever happens between 2 <see cref="TilePoint"/>s.
	/// </summary>
	public abstract class TilePointConnector {
		private TilePoint? from;
		private TilePoint? to;

		[AllowNull]
		public TilePoint From {
			get => from!;
			set {
				if ( from == value ) return;
				var old = from;
				from = value;

				if ( from is not null ) {
					if ( old is not null ) {
						var oldTo = To;
						old.ToNext = null;
						To = oldTo;
						From = value;
					}
					else {
						from.ToNext = this;
					}
				}
				else {
					old!.ToNext = null;
				}
			}
		}

		[AllowNull]
		public TilePoint To {
			get => to!;
			set {
				if ( to == value ) return;
				var old = to;
				to = value;

				if ( to is not null ) {
					if ( old is not null ) {
						var oldfrom = From;
						old.FromPrevious = null;
						From = oldfrom;
						To = value;
					}
					else {
						to.FromPrevious = this;
					}
				}
				else {
					old!.FromPrevious = null;
				}
			}
		}

		private int targetOrbitalIndex = 1;

		/// <summary>
		/// Which orbital from the current pivot should meet the <see cref="To"/> <see cref="TilePoint"/>.
		/// </summary>
		[Inspectable( Section = InspectableAttribute.SectionProperties, Label = "Target Orbital" )]
		public int TargetOrbitalIndex {
			get => targetOrbitalIndex;
			set {
				if ( targetOrbitalIndex == value ) return;

				targetOrbitalIndex = value;
				if ( To is not null ) // we are not initialzed yet
					Invalidate();
			}
		}

		/// <summary>
		/// Computes what position the <see cref="To"/> <see cref="TilePoint"/> should be at.
		/// </summary>
		public virtual Vector2d GetEndPosition ()
			=> GetEndState().PositionOfNth( TargetOrbitalIndex );

		public virtual Vector2d GetPositionAt ( double progress )
			=> GetStateAt( progress ).PositionOfNth( TargetOrbitalIndex );

		/// <summary>
		/// Computes what orbital state the <see cref="To"/> <see cref="TilePoint"/> should be at.
		/// </summary>
		public OrbitalState GetEndState ()
			=> GetStateAt( 1 );

		public abstract OrbitalState GetStateAt ( double progress );

		public double Duration => EndTime - StartTime;
		public double EndTime => To.StartTime;
		public double StartTime => From.StartTime;

		private double beatsPerMS;
		public double BPM { // TODO this is probably not updated in the editor
			get => beatsPerMS * 60000;
			set {
				beatsPerMS = value / 60000;
				Invalidate();
			}
		}

		[Inspectable( Section = InspectableAttribute.SectionTiming, Label = "Beat length" )]
		public double Beats => Duration * beatsPerMS;

		/// <summary>
		/// Force this <see cref="TilePointConnector"/> and any subsequent <see cref="TilePoint"/>s to recalcuate their properties such as <see cref="TilePoint.Position"/>.
		/// </summary>
		public void Invalidate () {
			InvalidateProperties();
			To.Invalidate();
		}

		/// <summary>
		/// Invalidates all properties of this <see cref="TilePointConnector"/>.
		/// </summary>
		protected virtual void InvalidateProperties () {
			
		}

		public virtual ConnectorBlueprint CreateEditorBlueprint () 
			=> new ConnectorBlueprint<TilePointConnector>( this );

		/// <summary>
		/// Apply sane defaults after being connected, such as keeping the same direction as the previous connector.
		/// </summary>
		public virtual void ApplyDefaults () { }

		#region editor formatting

		public static string FormatDistance ( double value )
			=> value == 1 ? "1 Tile" : $"{value:0.##} Tiles";
		public static double? ParseDistance ( string value ) {
			if ( value is null ) return null;

			if ( value.EndsWith( "Tile" ) )
				value = value.Substring( 0, value.Length - 4 );

			if ( value.EndsWith( "Tiles" ) )
				value = value.Substring( 0, value.Length - 5 );

			if ( double.TryParse( value.Trim(), out var d ) )
				return d;
			return null;
		}

		public static string FormatVelocity ( double value )
			=> value == 0.001 ? "1 Tile per second" : $"{value*1000:0.##} Tiles per second";
		public static double? ParseVelocity ( string value ) {
			if ( value is null ) return null;

			if ( value.EndsWith( "Tile per second" ) )
				value = value.Substring( 0, value.Length - 15 );

			if ( value.EndsWith( "Tiles per second" ) )
				value = value.Substring( 0, value.Length - 16 );

			if ( double.TryParse( value.Trim(), out var d ) )
				return d / 1000;
			else return null;
		}

		public static string FormatAngleRadiansToDegrees ( double value )
			=> $"{value.RadToDeg():N2}°";
		public static double? ParseDegreeAngleToRadians ( string value ) {
			if ( value is null ) return null;

			bool degrees = true;

			if ( value.EndsWith( "rad" ) ) {
				value = value.Substring( 0, value.Length - 3 );
				degrees = false;
			}
			else if ( value.EndsWith( "radians" ) ) {
				value = value.Substring( 0, value.Length - 7 );
				degrees = false;
			}
			else if ( value.EndsWith( "°" ) ) {
				value = value.Substring( 0, value.Length - 1 );
			}

			if ( double.TryParse( value.Trim(), out var d ) ) {
				if ( degrees )
					return d / 180 * Math.PI;
				else
					return d;
			}
			else return null;
		}

		public static string FormatMultiplier ( double value )
			=> $"x{value:N2}";
		public static double? ParseMultiplier ( string value ) {
			if ( value is null ) return null;

			if ( value.StartsWith( "x" ) )
				value = value.Substring( 1, value.Length - 1 );

			if ( value.EndsWith( "x" ) )
				value = value.Substring( 0, value.Length - 1 );

			if ( double.TryParse( value.Trim(), out var d ) )
				return d;
			else return null;
		}

		#endregion
	}
}
