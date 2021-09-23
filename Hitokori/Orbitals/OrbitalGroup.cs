using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.Orbitals {
	[Cached]
	public class OrbitalGroup : CompositeDrawable {
		BindableFloat animationProgress = new( 0 );
		public TilePoint CurrentTile { get; private set; }

		public override bool RemoveCompletedTransforms => false;

		public OrbitalGroup ( TilePoint currentTile ) {
			Origin = Anchor.Centre;
			Anchor = Anchor.Centre;
			CurrentTile = currentTile;
			AutoSizeAxes = Axes.Both;
			Alpha = 0;
		}

		[Resolved, MaybeNull, NotNull]
		private HitokoriPlayfield playfield { get; set; }

		private BindableFloat positionScale = new( HitokoriPlayfield.DefaultPositionScale );
		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );
			animationProgress.BindValueChanged( v => {
				foreach ( var i in activeOrbitals ) {
					i.Radius = v.NewValue;
				}
			} );
		}

		protected override void LoadComplete () {
			playfield.NewResult += onNewResult;

			var duration = ( CurrentTile.ToNext?.Duration * 2 ) ?? 500;
			using ( BeginAbsoluteSequence( CurrentTile.StartTime - duration ) ) {
				this.FadeIn( 150 )
					.Then().TransformBindableTo( animationProgress, 1, duration, Easing.Out );
			}
		}

		private void onNewResult ( DrawableHitObject dho, JudgementResult j ) {
			if ( dho.HitObject is TilePoint tile && tile.Next is null ) {

				using ( BeginAbsoluteSequence( tile.StartTime ) ) {
					this.TransformBindableTo( animationProgress, 0, 500, Easing.In )
						.Then().FadeOut( 150 )
						.Then().Expire();
				}
			}
		}

		public override bool IsPresent => true;
		public float NormalizedEnclosingCircleRadius { get; private set; }

		protected override void Update () {
			base.Update();

			while ( CurrentTile.FromPrevious is TilePointConnector fromPrevious && ( !playfield.TryGetResultFor( CurrentTile, out var j ) || j.TimeAbsolute >= Time.Current ) ) {
				CurrentTile = fromPrevious.From;
			}
			while ( CurrentTile.ToNext is TilePointConnector toNext && playfield.TryGetResultFor( toNext.To, out var value ) && value.TimeAbsolute <= Time.Current ) {
				updateState( toNext.GetEndState() );
				CurrentTile = toNext.To;
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
				var orbital = new Orbital( activeOrbitals.Count ) {
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
				var orbital = activeOrbitals[ i ];
				orbital.Position = (Vector2)( orbital.StateAt( Time.Current ).Position - state.PivotPosition ) * positionScale.Value;
			}

			NormalizedEnclosingCircleRadius = (float)state.EnclosingCircle.radius;
		}
	}
}
