using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Hitokori.Tests.TestingUI;
using osu.Game.Rulesets.Hitokori.UI.Visuals;
using osuTK;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.Tests.Visual {
	public class TestSceneTilePointVisual : MovableTestScene {
		DragablePoint from;
		DragablePoint posB;
		DragablePoint posA;
		DragablePoint to;

		BasicSliderBar<double> appearProgress;
		BasicSliderBar<double> outerProgress;
		BasicSliderBar<double> connectProgress;

		TilePointVisualPiece visualA;
		TilePointVisualPiece visualB;

		public TestSceneTilePointVisual () {
			Add( from = new DragablePoint() );
			Add( posA = new DragablePoint() );
			Add( posB = new DragablePoint() );
			Add( to = new DragablePoint() );

			AddOverlay( appearProgress = new BasicSliderBar<double> {
				Origin = Anchor.Centre,
				Anchor = Anchor.TopCentre,
				RelativeSizeAxes = Axes.X,
				Width = 0.9f,
				Y = 20,
				Height = 10,
				Current = new BindableDouble( 1 ) { MinValue = 0, MaxValue = 1 }
			} );

			AddOverlay( outerProgress = new BasicSliderBar<double> {
				Origin = Anchor.Centre,
				Anchor = Anchor.TopCentre,
				RelativeSizeAxes = Axes.X,
				Width = 0.9f,
				Y = 40,
				Height = 10,
				Current = new BindableDouble( 1 ) { MinValue = 0, MaxValue = 1 }
			} );

			AddOverlay( connectProgress = new BasicSliderBar<double> {
				Origin = Anchor.Centre,
				Anchor = Anchor.TopCentre,
				RelativeSizeAxes = Axes.X,
				Width = 0.9f,
				Y = 60,
				Height = 10,
				Current = new BindableDouble( 1 ) { MinValue = 0, MaxValue = 1 }
			} );

			Add( visualA = new TilePointVisualPiece { Anchor = Anchor.TopLeft, Depth = 1000 } );
			Add( visualB = new TilePointVisualPiece { Anchor = Anchor.TopLeft, Depth = 1000 } );

			from.Current.BindValueChanged( v => {
				visualA.FromPosition.Value = (Vector2d)( v.NewValue / visualA.PositionScale.Value );
			}, true );

			posA.Current.BindValueChanged( v => {
				visualA.AroundPosition.Value = (Vector2d)( v.NewValue / visualA.PositionScale.Value );
				visualB.FromPosition.Value = (Vector2d)( v.NewValue / visualA.PositionScale.Value );
				visualA.Position = v.NewValue;
			}, true );

			posB.Current.BindValueChanged( v => {
				visualA.ToPosition.Value = (Vector2d)( v.NewValue / visualA.PositionScale.Value );
				visualB.AroundPosition.Value = (Vector2d)( v.NewValue / visualA.PositionScale.Value );
				visualB.Position = v.NewValue;
			}, true );

			to.Current.BindValueChanged( v => {
				visualB.ToPosition.Value = (Vector2d)( v.NewValue / visualA.PositionScale.Value );
			}, true );

			appearProgress.Current.BindValueChanged( v => {
				visualA.Scale = visualB.Scale = new( (float)v.NewValue );
			}, true );
			outerProgress.Current.BindValueChanged( v => {
				visualA.InAnimationProgress.Value = (float)v.NewValue;
				visualB.OutAnimationProgress.Value = (float)v.NewValue;
			}, true );
			connectProgress.Current.BindValueChanged( v => {
				visualA.OutAnimationProgress.Value = (float)v.NewValue;
				visualB.InAnimationProgress.Value = (float)v.NewValue;
			}, true );

			from.Value = new Vector2( 100, 200 );
			posA.Value = new Vector2( 300, 200 );
			posB.Value = new Vector2( 300, 400 );
			to.Value = new Vector2( 500, 400 );
		}
	}
}
