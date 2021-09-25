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

namespace osu.Game.Rulesets.Hitokori.Tests.Collections {
	public class TestSceneTimelineSeeker : TestScene {
		TimelineSeeker<TimelineSeekerMarker> timeline = new( Comparer<TimelineSeekerMarker>.Create( (a,b) => 0 ) );
		Container container;
		BasicSliderBar<double> slider;

		public TestSceneTimelineSeeker () {
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

			timeline.EventStarted += e => e.Value.FadeColour( Colour4.Green ).Then().FlashColour( Colour4.DarkGreen, 300 );
			timeline.EventReverted += e => e.Value.FadeColour( Colour4.Green ).Then().FlashColour( Colour4.DarkGreen, 300 );
			timeline.EventEnded += e => e.Value.FadeColour( Colour4.White, 300 );
			timeline.EventRewound += e => e.Value.FadeColour( Colour4.White, 300 );

			int i = 0;

			for ( double t = RNG.NextDouble( 0.01, 0.05 ); t < 1; t += RNG.NextDouble( 0.01, 0.05 ) ) {
				double l;
				if ( RNG.NextBool( 0.8 ) ) {
					l = RNG.NextDouble( 0.05, 0.1 );
				}
				else {
					l = RNG.NextDouble( 0.1, 0.3 );
				}
				l = MathHelper.Clamp( l, 0, 1 - t );

				var marker = new TimelineSeekerMarker ( l ) {
					RelativePositionAxes = Axes.X,
					RelativeSizeAxes = Axes.X,
					X = (float)t,
					Y = 40 + i,
					Width = (float)l
				};

				i += 9;

				timeline.Add( t, l, marker );
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
				timeline.SeekTo( v.NewValue );
			} );
		}
	}

	public class TimelineSeekerMarker : CompositeDrawable {
		Circle circle;
		public double Length;

		public TimelineSeekerMarker ( double length ) {
			Length = length;

			Origin = Anchor.CentreLeft;
			Anchor = Anchor.CentreLeft;
			AutoSizeAxes = Axes.None;

			Height = 8;

			AddInternal( circle = new Circle {
				Origin = Anchor.CentreLeft,
				Anchor = Anchor.CentreLeft,
				RelativeSizeAxes = Axes.Both
			} );
		}
	}
}
