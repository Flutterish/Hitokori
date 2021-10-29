using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.UI.Paths;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Edit.SelectionOverlays {
	public class PathVisualizer : CompositeDrawable {
		public readonly Bindable<TilePointConnector?> VisualizedConnector = new();
		Bindable<double> timeBindable = new();
		public Vector2 TilePosition;

		FixedSizePath trail;

		public PathVisualizer () {
			Origin = Anchor.Centre;

			AddInternal( trail = new FixedSizePath { 
				PathRadius = 0.05f,
				Alpha = 0.6f
			} );

			VisualizedConnector.BindValueChanged( v => {
				ClearTransforms();

				if ( v.NewValue is not null ) {
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
			}

			trail.Position = -trail.PositionInBoundingBox( Vector2.Zero );
		}

		private static Vector2 positionAt ( TilePointConnector c, double t ) {
			var state = c.Duration == 0 ? c.GetEndState() : c.GetStateAt( t / c.Duration );
			return (Vector2)state.PositionOfNth( c.TargetOrbitalIndex );
		}
	}
}
