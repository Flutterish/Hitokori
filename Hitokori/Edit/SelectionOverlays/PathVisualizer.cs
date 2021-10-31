using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Hitokori.UI.Paths;
using osuTK;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Edit.SelectionOverlays {
	public class PathVisualizer : CompositeDrawable {
		[Resolved, MaybeNull, NotNull]
		public HitokoriPlayfield Playfield { get; private set; }

		public readonly Bindable<TilePointConnector?> VisualizedConnector = new();
		Bindable<double> timeBindable = new();
		public Vector2 TilePosition;

		FixedSizePath trail;
		private List<Drawable> heads = new();

		public PathVisualizer () {
			Origin = Anchor.Centre;
			AutoSizeAxes = Axes.Both;

			AddInternal( trail = new FixedSizePath { 
				PathRadius = 0.05f,
				Alpha = 0.6f,
				Anchor = Anchor.Centre
			} );

			VisualizedConnector.BindValueChanged( v => {
				ClearTransforms();

				if ( v.NewValue is not null ) {
					Duration = 0;
					Show();
				}
				else {
					Hide();
				}
			} );
		}

		private double duration;
		private double Duration {
			get => duration;
			set {
				if ( duration == value ) return;

				duration = value;
				ClearTransforms();
				if ( duration <= 0 ) return;
				this.Loop( 300, x => x.TransformBindableTo( timeBindable, 0 ).Then().TransformBindableTo( timeBindable, value * 2, value * 2 ) );
			}
		}
		private double startTime => VisualizedConnector.Value!.From.Previous is null ? (VisualizedConnector.Value.StartTime - VisualizedConnector.Value.Duration * 2) : (VisualizedConnector.Value.StartTime + 0.1);

		protected override void Update () {
			base.Update();

			if ( VisualizedConnector.Value is not TilePointConnector c ) return;
			var d = c.EndTime - startTime;
			Duration = d > 100 ? (d - 0.1) : (d * 0.999); // this is done so the small jump that OrbitalState.Offset produces is not shown

			trail.ClearVertices();

			if ( Duration <= 0 ) {
				foreach ( var i in heads )
					i.Hide();

				trail.AddVertex( (Vector2)c.GetStateAt( 0 ).PositionOfNth( c.TargetOrbitalIndex ) - TilePosition );
				trail.AddVertex( (Vector2)c.GetStateAt( 1 ).PositionOfNth( c.TargetOrbitalIndex ) - TilePosition );
			}
			else {
				var orbitals = Playfield.ChainWithID( c.To.ChainID );

				const int count = 100;
				int start = 0;
				int end = count;

				if ( timeBindable.Value < Duration ) {
					end = (int)( ( timeBindable.Value / Duration ) * count );
				}
				else {
					start = (int)( ( timeBindable.Value / Duration - 1 ) * count );
				}

				var state = c.GetStateAt( 0 );
				
				void addVertex ( double time ) {
					orbitals!.SeekTo( time );
					trail.AddVertex( (Vector2)orbitals.ActiveOrbitals.Single( x => state.IsNthOriginalPivot( x.Index - c.TargetOrbitalIndex ) ).StateAt( time ).Position - TilePosition );
				}
				
				addVertex( startTime + ( ( timeBindable.Value < Duration ) ? 0 : ( timeBindable.Value - Duration ) ) );
				for ( int i = start; i < end; i++ ) {
					addVertex( startTime + ( Duration / ( count - 1 ) ) * i );
				}

				var endTime = startTime + ( ( timeBindable.Value < Duration ) ? timeBindable.Value : duration );
				addVertex( endTime );

				foreach ( var i in heads )
					i.Hide();

				int k = 0;
				foreach ( var i in orbitals.ActiveOrbitals ) {
					if ( heads.Count <= k ) {
						var head = new Circle {
							Origin = Anchor.Centre,
							Anchor = Anchor.Centre,
							Size = new Vector2( 0.3f )
						};

						heads.Add( head );
						AddInternal( head );
					}

					var orbitalState = i.StateAt( endTime );
					heads[ k ].Show();
					heads[ k ].Position = (Vector2)orbitalState.Position - TilePosition;
					heads[ k ].Scale = i.ScaleForZ( orbitalState.Z );

					k++;
				}
			}

			trail.Position = -trail.PositionInBoundingBox( Vector2.Zero );
		}
	}
}
