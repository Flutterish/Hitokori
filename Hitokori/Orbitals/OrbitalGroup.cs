using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.Orbitals {
	public class OrbitalGroup : CompositeDrawable {
		BindableFloat animationProgress = new( 0 );

		public OrbitalGroup ( TilePoint currentTile ) {
			Origin = Anchor.Centre;
			Anchor = Anchor.Centre;
			CurrentTile = currentTile;
			AutoSizeAxes = Axes.Both;
			Alpha = 0;
			updateState( currentTile.OrbitalState );
		}

		private BindableFloat positionScale = new( HitokoriPlayfield.DefaultPositionScale );
		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );
		}

		protected override void LoadComplete () {
			var duration = (CurrentTile.ToNext?.Duration * 2) ?? 500;

			using ( BeginAbsoluteSequence( CurrentTile.StartTime - duration ) ) {
				this.FadeIn( 150 )
					.Then().TransformBindableTo( animationProgress, 1, duration, Easing.Out );
			}
		}

		public override bool IsPresent => true;
		public TilePoint CurrentTile { get; private set; }
		public float NormalizedEnclosingCircleRadius { get; private set; }

		protected override void Update () {
			base.Update();

			while ( CurrentTile.ToNext is TilePointConnector connector && connector.To.StartTime <= Time.Current ) {
				updateState( connector.GetEndState() );
				CurrentTile = connector.To;

				if ( CurrentTile.Next is null ) {
					using ( BeginAbsoluteSequence( CurrentTile.StartTime ) ) {
						this.TransformBindableTo( animationProgress, 0, 500, Easing.In )
							.Then().FadeOut( 150 )
							.Then().Expire();
					}
				}
			}

			if ( CurrentTile.ToNext is TilePointConnector c ) {
				updateState( c.Duration == 0 ? c.GetEndState() : c.GetStateAt( ( Time.Current - c.StartTime ) / c.Duration ) );
			}
			else if ( CurrentTile.FromPrevious is TilePointConnector p ) {
				updateState( CurrentTile.ModifyOrbitalState( p.Duration == 0 ? p.GetEndState() : p.GetStateAt( ( Time.Current - p.StartTime ) / p.Duration ) ) );
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
			Alpha = animationProgress.Value;

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

			Position = (Vector2)state.PivotPosition * positionScale.Value;

			for ( int i = 0; i < activeOrbitals.Count; i++ ) {
				activeOrbitals[ i ].Position = (Vector2)state.OffsetOfNthOriginal( i ) * positionScale.Value * animationProgress.Value;
			}

			NormalizedEnclosingCircleRadius = (float)state.EnclosingCircle.radius * animationProgress.Value;
		}
	}
}
