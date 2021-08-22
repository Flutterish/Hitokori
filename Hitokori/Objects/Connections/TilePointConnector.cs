using osu.Game.Rulesets.Hitokori.Orbitals;
using osuTK;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.Objects {
	/// <summary>
	/// A rotation, linear movement or whatever happens between 2 <see cref="TilePoint"/>s.
	/// </summary>
	public abstract class TilePointConnector {
		private TilePoint from = TilePoint.Unit;
		private TilePoint to = TilePoint.Unit;

		public TilePoint From {
			get => from;
			set {
				if ( from == value ) return;
				var old = from;
				from = value;
				
				if ( old.ToNext == this ) old.ToNext = null;
				from.ToNext = this;
			}
		}
		public TilePoint To {
			get => to;
			set {
				if ( to == value ) return;
				var old = to;
				to = value;

				if ( old.FromPrevious == this ) old.FromPrevious = null;
				to.FromPrevious = this;
			}
		}

		private int targetOrbitalIndex = 1;

		/// <summary>
		/// Which orbital from the current pivot should meet the <see cref="To"/> <see cref="TilePoint"/>.
		/// </summary>
		public int TargetOrbitalIndex {
			get => targetOrbitalIndex;
			set {
				if ( targetOrbitalIndex == value ) return;

				targetOrbitalIndex = value;
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

		private float beatsPerMS;
		public float BPM {
			get => beatsPerMS * 60000;
			set {
				beatsPerMS = value / 60000;
				Invalidate();
			}
		}

		public float Beats => (float)Duration * beatsPerMS;

		/// <summary>
		/// Force this <see cref="TilePointConnector"/> and any subsequent <see cref="TilePoint"/>s to recalcuate their properties such as <see cref="TilePoint.Position"/>.
		/// </summary>
		protected void Invalidate () {
			InvalidateProperties();
			To.Invalidate();
		}

		/// <summary>
		/// Invalidates all properties of this <see cref="TilePointConnector"/>.
		/// </summary>
		protected virtual void InvalidateProperties () {
			
		}
	}
}
