using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.Orbitals {
	public class OrbitalGroup : CompositeDrawable {
		public OrbitalGroup ( TilePoint currentTile ) {
			Origin = Anchor.Centre;
			Anchor = Anchor.Centre;
			CurrentTile = currentTile;
			AutoSizeAxes = Axes.Both;
		}

		public TilePoint CurrentTile;

		protected override void Update () {
			base.Update();

			while ( CurrentTile.ToNext is TilePointConnector connector && connector.To.StartTime <= Time.Current ) {
				updateState( connector.GetEndState() );
				CurrentTile = connector.To;
			}

			if ( CurrentTile.ToNext is TilePointConnector c ) {
				updateState( c.GetStateAt( ( Time.Current - c.StartTime ) / c.Duration ) );
			}
		}

		private List<Orbital> activeOrbitals = new List<Orbital>();
		private Color4[] orbitalColors = new Color4[] {
			Color4.Red,
			Color4.Blue,
			Color4.Green,
			Color4.Magenta,
			Color4.Orange,
			Color4.HotPink,
			Color4.Cyan,
			Color4.Fuchsia,
		};

		void updateState ( OrbitalState state ) {
			while ( activeOrbitals.Count < state.OrbitalCount ) {
				var orbital = new Orbital {
					Colour = orbitalColors[ activeOrbitals.Count % orbitalColors.Length ]
				};
				activeOrbitals.Add( orbital );
				AddInternal( orbital );
			}
			while ( activeOrbitals.Count > state.OrbitalCount ) {
				var last = activeOrbitals[ activeOrbitals.Count - 1 ];
				activeOrbitals.RemoveAt( activeOrbitals.Count - 1 );
				RemoveInternal( last );
			}

			Position = (Vector2)state.PivotPosition * 100;

			for ( int i = 0; i < activeOrbitals.Count; i++ ) {
				activeOrbitals[ i ].Position = (Vector2)state.OffsetOfNthOriginal( i ) * 100;
			}
		}
	}
}
