using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Orbitals.Events;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using System;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Orbitals {
	public class Orbital : CompositeDrawable {
		private Circle circle;
		private Trail trail;

		public int Index;

		public Orbital ( int index ) {
			Index = index;

			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AddInternal( trail = new Trail() );
			AddInternal( circle = new Circle {
				Size = new Vector2( 20 ),
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre
			} );

			currentState = new VisualOrbitalState {
				Alpha = 0,
				Index = index
			};

			AlwaysPresent = true;
			Alpha = 0;
		}

		private double verticeInterval = 8;
		private double accumulatedTime;

		protected override bool ComputeIsMaskedAway ( RectangleF maskingBounds )
			=> false;

		protected override void Update () {
			base.Update();

			updateTrail();
		}

		private void updateTrail () {
			if ( group is null ) {
				accumulatedTime += Time.Elapsed;
				trail.Offset = Position + Parent.Position;
				while ( accumulatedTime >= verticeInterval ) {
					accumulatedTime -= verticeInterval;

					trail.AddVertice( trail.Offset );
				}
			}
			else {
				trail.Offset = Position + Parent.Position;
				for ( int i = trail.VerticeCount - 1; i > 0; i-- ) {
					trail.AddVertice( (Vector2)StateAt( Math.Round( Time.Current / verticeInterval - i ) * verticeInterval ).Position * positionScale.Value );
				}
				trail.AddVertice( (Vector2)StateAt( Time.Current ).Position * positionScale.Value );
			}
		}

		[Resolved(canBeNull: true)]
		private OrbitalGroup? group { get; set; }
		[Resolved(canBeNull: true)]
		private HitokoriPlayfield? playfield { get; set; }

		private BindableFloat positionScale = new( HitokoriPlayfield.DefaultPositionScale );
		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );

			if ( group is null ) {
				positionScale.BindValueChanged( v => {
					trail.Rescale( v.NewValue / v.OldValue );
				} );
			}
			else {
				currentTile = group.CurrentTile;
				positionScale.BindValueChanged( v => {
					updateTrail();
					trail.FlushVertices();
				} );
			}

			simulatedTime = Time.Current;

			if ( playfield is not null ) {
				playfield.NewResult += onNewResult;
			}

			VisualEvents.TimeSeeked += t => {
				simulateTo( t );
			};
		}

		private void onNewResult ( DrawableHitObject dho, JudgementResult j ) {
			var tile = dho.HitObject as TilePoint;
			if ( tile is null ) return;

			if ( tile.StartTime <= simulatedTime ) {
				simulateBackwardTo( tile.StartTime );
			}

			// TODO add hit events
		}

		private VisualOrbitalState currentState;
		[NotNull, MaybeNull]
		public TilePoint currentTile;
		private double simulatedTime;

		public double Radius = 0;
		public readonly VisualEventSeeker VisualEvents = new();

		public VisualOrbitalState StateAt ( double time ) {
			VisualEvents.CurrentTime = time;
			VisualEvents.Apply();

			return simulateTo( time );
		}

		private VisualOrbitalState simulateTo ( double time ) {
			if ( time < simulatedTime ) {
				return simulateBackwardTo( time );
			}
			else if ( time > simulatedTime ) {
				return simulateForwardTo( time );
			}

			return currentState;
		}

		private VisualOrbitalState simulateForwardTo ( double time ) {
			if ( playfield is null ) {
				while ( currentTile.ToNext is TilePointConnector toNext && toNext.EndTime <= time ) {
					currentTile = toNext.To;
				}
			}
			else {
				while ( currentTile.ToNext is TilePointConnector toNext && playfield.TryGetResultFor( toNext.To, out var j ) && j.TimeAbsolute <= time ) {
					currentTile = toNext.To;
				}
			}

			simulatedTime = time;

			if ( currentTile.ToNext is TilePointConnector c ) {
				updateState( c.Duration == 0 ? c.GetEndState() : c.GetStateAt( ( time - c.StartTime ) / c.Duration ) );
			}
			else if ( currentTile.FromPrevious is TilePointConnector p ) {
				updateState( currentTile.ModifyOrbitalState( p.Duration == 0 ? p.GetEndState() : p.GetStateAt( ( time - p.StartTime ) / p.Duration ) ) );
			}

			return currentState;
		}

		private VisualOrbitalState simulateBackwardTo ( double time ) {
			if ( playfield is null ) {
				while ( currentTile.FromPrevious is TilePointConnector fromPrevious && currentTile.StartTime >= time ) {
					currentTile = fromPrevious.From;
				}
			}
			else {
				while ( currentTile.FromPrevious is TilePointConnector fromPrevious && ( !playfield.TryGetResultFor( currentTile, out var j ) || j.TimeAbsolute >= time ) ) {
					currentTile = fromPrevious.From;
				}
			}

			simulatedTime = time;

			if ( currentTile.ToNext is TilePointConnector c ) {
				updateState( c.Duration == 0 ? c.GetStateAt( 0 ) : c.GetStateAt( ( time - c.StartTime ) / c.Duration ) );
			}
			else if ( currentTile.FromPrevious is TilePointConnector p ) {
				updateState( currentTile.ModifyOrbitalState( p.Duration == 0 ? p.GetStateAt( 0 ) : p.GetStateAt( ( time - p.StartTime ) / p.Duration ) ) );
			}

			return currentState;
		}

		private void updateState ( OrbitalState state ) {
			currentState.Position = state.PivotPosition + state.OffsetOfNthOriginal( Index ) * Radius;
			currentState.Alpha = Alpha;
		}
	}
}
