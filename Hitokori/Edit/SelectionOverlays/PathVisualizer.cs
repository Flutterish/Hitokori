using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.UI.Paths;
using osuTK;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Edit.SelectionOverlays {
	public class PathVisualizer : CompositeDrawable {
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

		protected override void Update () {
			base.Update();

			if ( VisualizedConnector.Value is null ) return;
			Duration = VisualizedConnector.Value.Duration;

			trail.ClearVertices();

			if ( Duration == 0 ) {
				foreach ( var i in heads )
					i.Hide();

				trail.AddVertex( (Vector2)VisualizedConnector.Value.GetStateAt( 0 ).PositionOfNth( VisualizedConnector.Value.TargetOrbitalIndex ) - TilePosition );
				trail.AddVertex( (Vector2)VisualizedConnector.Value.GetStateAt( 1 ).PositionOfNth( VisualizedConnector.Value.TargetOrbitalIndex ) - TilePosition );
			}
			else {
				const int count = 100;
				int start = 0;
				int end = count;

				if ( timeBindable.Value < Duration ) {
					end = (int)( ( timeBindable.Value / Duration ) * count );
				}
				else {
					start = (int)( ( timeBindable.Value / Duration - 1 ) * count );
				}

				for ( int i = start; i < end; i++ ) {
					var t = ( Duration / ( count - 1 ) ) * i;
					trail.AddVertex( positionAt( VisualizedConnector.Value, t ) - TilePosition );
				}

				var endState = VisualizedConnector.Value.GetStateAt( ( 1.0 / ( count - 1 ) ) * (end-1) );
				foreach ( var i in heads )
					i.Hide();

				for ( int i = 0; i < endState.OrbitalCount; i++ ) {
					if ( heads.Count <= i ) {
						var head = new Circle {
							Origin = Anchor.Centre,
							Anchor = Anchor.Centre,
							Size = new Vector2( 0.3f )
						};

						heads.Add( head );
						AddInternal( head );
					}

					heads[ i ].Show();
					heads[ i ].Position = (Vector2)endState.PositionOfNth( i ) - TilePosition;
				}
			}

			trail.Position = -trail.PositionInBoundingBox( Vector2.Zero );
		}

		private static Vector2 positionAt ( TilePointConnector c, double t ) {
			return (Vector2)c.GetStateAt( t / c.Duration ).PositionOfNth( c.TargetOrbitalIndex );
		}
	}
}
