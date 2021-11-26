using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Testing;
using osu.Game.Rulesets.Hitokori.Collections;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Tests.Collections
{
    public class TestSceneTimelineSeeker : TestScene
    {
        TimelineSeeker<TimelineSeekerMarker> timeline = new(Comparer<TimelineSeekerMarker>.Create((a, b) => a.Order - b.Order));
        Container container;
        BasicSliderBar<double> slider;
        Container<TimelineSeekerMarker> markerContainer;

        public TestSceneTimelineSeeker()
        {
            Add(container = new Container
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Width = 0.9f
            });

            container.Add(slider = new BasicSliderBar<double>
            {
                Current = new BindableDouble { MinValue = 0, MaxValue = 1, Precision = 0.005 },
                Origin = Anchor.TopCentre,
                Anchor = Anchor.TopCentre,
                Y = 10,
                Height = 30,
                RelativeSizeAxes = Axes.X
            });

            container.Add(new Circle
            {
                Origin = Anchor.TopCentre,
                Anchor = Anchor.TopCentre,
                RelativeSizeAxes = Axes.X,
                Y = 45,
                Height = 8
            });

            container.Add(markerContainer = new Container<TimelineSeekerMarker>
            {
                Origin = Anchor.TopCentre,
                Anchor = Anchor.TopCentre,
                RelativeSizeAxes = Axes.X,
                Y = 55
            });

            timeline.EventStarted += e =>
            {
                e.Value.IsActive = true;
            };
            timeline.EventReverted += e =>
            {
                e.Value.IsActive = true;
            };
            timeline.EventEnded += e =>
            {
                e.Value.IsActive = false;
            };
            timeline.EventRewound += e =>
            {
                e.Value.IsActive = false;
            };

            slider.Current.BindValueChanged(v =>
            {
                timeline.SeekTo(v.NewValue);
            });
        }

        new public void Clear()
        {
            markerContainer.Clear();
            timeline.Clear();
        }

        public void Add(double start, double duration)
        {
            var entries = timeline.EntriesBetween(start, start + duration);
            int y = 0;
            while (entries.Any(x => x.Value.HeightOffset == y)) y++;

            var marker = new TimelineSeekerMarker(duration)
            {
                RelativePositionAxes = Axes.X,
                RelativeSizeAxes = Axes.X,
                X = (float)start,
                Y = (y + 1) * 40,
                Width = (float)duration,
                Order = timeline.EntriesAt(start).Count(),
                HeightOffset = y
            };

            timeline.Add(start, duration, marker);
            markerContainer.Add(marker);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            double gap = 0.025;
            AddStep("Populate Timeline", () =>
            {
                Clear();
                slider.Current.Value = 0;

                double time = 0.01;

                /* [ 0  ] */
                Add(time, 0.05);
                time += 0.05 + gap;

                /* [ 1  ] */
                Add(time, 0.1);
                time += 0.05 - 0.025;
                /* [ 2  ] */
                Add(time, 0.05);
                time += 0.075 + gap;

                /* [ 3  ] */
                Add(time, 0.075);
                time += 0.05;
                /* [ 4  ] */
                Add(time, 0.1);
                time += 0.05 + 0.025;
                /* [ 5  ] */
                Add(time, 0.075);
                time += 0.075 + gap;

                /* [ 6  ] */
                Add(time, 0.1);
                time += 0.1;
                /* [ 7  ] */
                Add(time, 0.1);
                time += 0.1 + gap;

                /* [ 8  ] */
                Add(time, 0.1);
                /* [ 9  ] */
                Add(time, 0.075);
                /* [ 10 ] */
                Add(time, 0.05);
                time += 0.1 + gap;

                /* [ 11 ] */
                Add(time, 0.1);
                /* [ 12 ] */
                Add(time + 0.025, 0.075);
                /* [ 13 ] */
                Add(time + 0.05, 0.05);
            });

            AddLabel("Single");
            AddStep("Move into", () => slider.Current.Value = timeline[0].StartTime + timeline[0].Duration / 2);
            AddAssert("Entry 0 Active", () => timeline[0].Value.IsActive == true);
            AddStep("Move past", () => slider.Current.Value = timeline[0].EndTime + gap / 2);
            AddAssert("Entry 0 Not Active", () => timeline[0].Value.IsActive == false);
            AddStep("Move before", () => slider.Current.Value = timeline[0].StartTime - gap / 2);
            AddAssert("Entry 0 Not Active", () => timeline[0].Value.IsActive == false);
            AddStep("Move to beginning", () => slider.Current.Value = timeline[0].StartTime);
            AddAssert("Entry 0 Active", () => timeline[0].Value.IsActive == true);
            AddStep("Move into", () => slider.Current.Value = timeline[0].StartTime + timeline[0].Duration / 2);
            AddAssert("Entry 0 Active", () => timeline[0].Value.IsActive == true);
            AddStep("Move to end", () => slider.Current.Value = timeline[0].EndTime);
            AddAssert("Entry 0 Active", () => timeline[0].Value.IsActive == true);
            AddStep("Move past", () => slider.Current.Value = timeline[0].EndTime + gap / 2);
            AddAssert("Entry 0 Not Active", () => timeline[0].Value.IsActive == false);
            AddStep("Move to end", () => slider.Current.Value = timeline[0].EndTime);
            AddAssert("Entry 0 Active", () => timeline[0].Value.IsActive == true);
            AddStep("Move into", () => slider.Current.Value = timeline[0].StartTime + timeline[0].Duration / 2);
            AddAssert("Entry 0 Active", () => timeline[0].Value.IsActive == true);
            AddStep("Move to beginning", () => slider.Current.Value = timeline[0].StartTime);
            AddAssert("Entry 0 Active", () => timeline[0].Value.IsActive == true);
            AddStep("Move before", () => slider.Current.Value = timeline[0].StartTime - gap / 2);
            AddAssert("Entry 0 Not Active", () => timeline[0].Value.IsActive == false);

            AddLabel("TODO"); // TODO
        }
    }

    public class TimelineSeekerMarker : CompositeDrawable
    {
        public int Order;
        public int HeightOffset;
        Circle circle;
        public double Length;
        private bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive == value) return;

                isActive = value;
                if (isActive)
                    this.FadeColour(Colour4.Green).Then().FlashColour(Colour4.DarkGreen, 300);
                else
                    this.FadeColour(Colour4.White, 300);
            }
        }

        public TimelineSeekerMarker(double length)
        {
            Length = length;

            Origin = Anchor.CentreLeft;
            Anchor = Anchor.CentreLeft;
            AutoSizeAxes = Axes.None;

            Height = 8;

            AddInternal(circle = new Circle
            {
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Both
            });
        }
    }
}
