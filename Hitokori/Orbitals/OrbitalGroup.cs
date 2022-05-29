﻿using osu.Framework.Allocation;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Orbitals.Events;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osuTK.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Orbitals {
	[Cached]
	public class OrbitalGroup : CompositeDrawable {
		public TilePoint FirstTile { get; private set; }
		public TilePoint LastTile { get; private set; }
		public TilePoint CurrentTile { get; private set; }

		public override bool RemoveCompletedTransforms => false;

		public OrbitalGroup ( TilePoint currentTile ) {
			Origin = Anchor.Centre;
			Anchor = Anchor.Centre;
			CurrentTile = currentTile;
			FirstTile = currentTile.First;
			LastTile = currentTile.Last;
			AutoSizeAxes = Axes.Both;
		}

		[Resolved, MaybeNull, NotNull]
		private HitokoriPlayfield playfield { get; set; }

		private BindableFloat positionScale = new( HitokoriPlayfield.DefaultPositionScale );
		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );

			positionScale.ValueChanged += _ => SeekTo( Time.Current );
		}

		public override bool IsPresent => true;
		public float NormalizedEnclosingCircleRadius { get; private set; }

		protected override void Update () {
			base.Update();

			SeekTo( Time.Current );
		}

		public void SeekTo ( double time ) {
			while ( CurrentTile.FromPrevious is TilePointConnector fromPrevious && ( !playfield.TryGetResultFor( CurrentTile, out var j ) || j.TimeAbsolute >= time ) ) {
				CurrentTile = fromPrevious.From;
				updateState( CurrentTile.OrbitalState );
			}
			while ( CurrentTile.ToNext is TilePointConnector toNext && playfield.TryGetResultFor( toNext.To, out var value ) && value.TimeAbsolute <= time ) {
				CurrentTile = toNext.To;
				updateState( CurrentTile.OrbitalState );
			}

			if ( CurrentTile.ToNext is TilePointConnector c ) {
				updateState( c.Duration == 0 ? c.GetEndState() : c.GetStateAt( ( time - c.StartTime ) / c.Duration ) );
			}
			else if ( CurrentTile.FromPrevious is TilePointConnector p ) {
				updateState( CurrentTile.ModifyOrbitalState( p.Duration == 0 ? p.GetEndState() : p.GetStateAt( ( time - p.StartTime ) / p.Duration ) ) );
			}
		}

		public IEnumerable<Orbital> ActiveOrbitals => activeOrbitals;
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
				var orbital = new Orbital( activeOrbitals.Count ) {
					Colour = orbitalColors[ activeOrbitals.Count % orbitalColors.Length ]
				};
				activeOrbitals.Add( orbital );
				AddInternal( orbital );

				var duration = ( FirstTile.ToNext?.Duration * 2 ) ?? 500;
				orbital.VisualEvents.Add( new FadeVisualEvent( orbital, 1, FirstTile.StartTime - duration, 150 ) );
				orbital.VisualEvents.Add( new ChangeRadiusVisualEvent( orbital, 1, FirstTile.StartTime - duration + 150, duration, Easing.Out ) );

				orbital.VisualEvents.Add( new ChangeRadiusVisualEvent( orbital, 0, LastTile.StartTime, 500, Easing.In ) );
				orbital.VisualEvents.Add( new FadeVisualEvent( orbital, 0, LastTile.StartTime + 450, 150 ) );
			}
			while ( activeOrbitals.Count > state.OrbitalCount ) {
				var last = activeOrbitals[ activeOrbitals.Count - 1 ];
				activeOrbitals.RemoveAt( activeOrbitals.Count - 1 );
				RemoveInternal( last );
			}

			Position = (Vector2)state.PivotPosition * positionScale.Value;

			for ( int i = 0; i < activeOrbitals.Count; i++ ) {
				var orbital = activeOrbitals[ i ];
				orbital.Position = (Vector2)( orbital.StateAt( Time.Current ).Position - state.PivotPosition ) * positionScale.Value;
			}

			NormalizedEnclosingCircleRadius = (float)state.EnclosingCircle.radius;
		}
	}
}
