using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Testing;
using osu.Framework.Utils;
using osu.Game.Rulesets.Hitokori.Collections;
using osuTK;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Tests.Collections {
	public class TestSceneTimeline : TestScene {
		Timeline<TimelineMarker> timeline = new( Comparer<TimelineMarker>.Create( (a,b) => 0 ) );
		Container container;
		BasicSliderBar<double> slider;

		public TestSceneTimeline () {
			Add( container = new Container {
				Origin = Anchor.Centre,
				Anchor = Anchor.Centre,
				RelativeSizeAxes = Axes.Both,
				Width = 0.9f
			} );

			container.Add( new Circle {
				Origin = Anchor.Centre,
				Anchor = Anchor.Centre,
				RelativeSizeAxes = Axes.X,
				Height = 8
			} );

			for ( double t = RNG.NextDouble( 0.01, 0.05 ); t < 1; t += RNG.NextDouble( 0.01, 0.05 ) ) {
				var marker = new TimelineMarker {
					RelativePositionAxes = Axes.X,
					X = (float)t,
					Y = 40
				};

				timeline.Add( t, marker );
				container.Add( marker );
			}

			container.Add( slider = new BasicSliderBar<double> {
				Current = new BindableDouble { MinValue = 0, MaxValue = 1 },
				Origin = Anchor.Centre, 
				Anchor = Anchor.Centre,
				Y = -100,
				Height = 30,
				RelativeSizeAxes = Axes.X
			} );

			slider.Current.BindValueChanged( v => {
				foreach ( var i in container.Children.OfType<TimelineMarker>() ) {
					i.Colour = Colour4.White;
				}

				var prev = timeline.FirstBefore( v.NewValue );
				var next = timeline.FirstAfter( v.NewValue );

				if ( prev >= 0 ) {
					container.Children.OfType<TimelineMarker>().ElementAt( prev ).Colour = Colour4.Red;
				}

				if ( next >= 0 ) {
					container.Children.OfType<TimelineMarker>().ElementAt( next ).Colour = Colour4.Green;
				}
			} );
		}
	}

	public class TimelineMarker : CompositeDrawable {
		public TimelineMarker () {
			Origin = Anchor.BottomCentre;
			Anchor = Anchor.CentreLeft;

			AddInternal( new Circle {
				Origin = Anchor.Centre,
				Anchor = Anchor.Centre,
				Size = new Vector2( 8 )
			} );
		}
	}
}
