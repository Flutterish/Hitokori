using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Testing;
using osu.Game.Rulesets.Hitokori.Collections;
using osuTK;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Tests.Collections
{
    public class TestSceneTimeline : TestScene
    {
        Timeline<TimelineMarker> timeline = new(Comparer<TimelineMarker>.Create((a, b) => a.Order - b.Order));
        Container container;
        BasicSliderBar<double> slider;
        Container<TimelineMarker> markerContainer;

        public TestSceneTimeline()
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

            container.Add(markerContainer = new Container<TimelineMarker>
            {
                Origin = Anchor.TopCentre,
                Anchor = Anchor.TopCentre,
                RelativeSizeAxes = Axes.X,
                Y = 55
            });

            slider.Current.BindValueChanged(v =>
            {
                foreach (var i in markerContainer)
                {
                    i.LabelText = "";
                }

                int index = timeline.FirstBefore(v.NewValue);
                if (index != -1)
                {
                    timeline[index].Value.LabelText += string.IsNullOrEmpty(timeline[index].Value.LabelText)
                        ? "<"
                        : " | <";
                }

                index = timeline.FirstAfter(v.NewValue);
                if (index != -1)
                {
                    timeline[index].Value.LabelText += string.IsNullOrEmpty(timeline[index].Value.LabelText)
                        ? ">"
                        : " | >";
                }

                index = timeline.FirstBeforeOrAt(v.NewValue);
                if (index != -1)
                {
                    timeline[index].Value.LabelText += string.IsNullOrEmpty(timeline[index].Value.LabelText)
                        ? "≤"
                        : " | ≤";
                }

                index = timeline.FirstAfterOrAt(v.NewValue);
                if (index != -1)
                {
                    timeline[index].Value.LabelText += string.IsNullOrEmpty(timeline[index].Value.LabelText)
                        ? "≥"
                        : " | ≥";
                }

                foreach (var i in markerContainer)
                {
                    if (i.LabelText == "≤ | ≥") i.LabelText = "=";
                }

                index = timeline.LastAtOrFirstBefore(v.NewValue);
                if (index != -1 && timeline[index].Value.LabelText == "")
                {
                    timeline[index].Value.LabelText = "≈";
                }
            });
        }

        new public void Clear()
        {
            markerContainer.Clear();
            timeline.Clear();
        }

        public void Add(double progress)
        {
            int y = timeline.EntriesAt(progress).Count();

            var marker = new TimelineMarker
            {
                X = (float)progress,
                Y = (y + 1) * 40,
                Order = y
            };

            timeline.Add(progress, marker);
            markerContainer.Add(marker);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddStep("Populate Timeline", () =>
            {
                Clear();
                /* [ 0  ] */
                Add(0.01);
                /* [ 1  ] */
                Add(0.05);
                /* [ 2  ] */
                Add(0.1);
                /* [ 3  ] */
                Add(0.1);
                /* [ 4  ] */
                Add(0.1);
                /* [ 5  ] */
                Add(0.2);
                /* [ 6  ] */
                Add(0.55);
                /* [ 7  ] */
                Add(0.75);
                /* [ 8  ] */
                Add(0.85);
                /* [ 9  ] */
                Add(0.9);
                /* [ 10 ] */
                Add(0.9);
                /* [ 11 ] */
                Add(0.99);
            });

            (int before, int beforeOrAt, int after, int afterOrAt) testIndexes() => (
                timeline.FirstBefore(slider.Current.Value),
                timeline.FirstBeforeOrAt(slider.Current.Value),
                timeline.FirstAfter(slider.Current.Value),
                timeline.FirstAfterOrAt(slider.Current.Value)
            );

            AddLabel("Beginning");
            AddStep("Move to t = 0", () =>
            {
                slider.Current.Value = 0;
            });
            AddAssert("No entries before", () => testIndexes() is (before: -1, beforeOrAt: -1, after: _, afterOrAt: _));
            AddAssert("Entry 0 after", () => testIndexes() is (before: _, beforeOrAt: _, after: 0, afterOrAt: 0));


            AddLabel("At First Entry");
            AddStep("Move to t = 0.01", () =>
            {
                slider.Current.Value = 0.01;
            });
            AddAssert("No entries before", () => testIndexes() is (before: -1, beforeOrAt: _, after: _, afterOrAt: _));
            AddAssert("At Entry 0", () => testIndexes() is (before: _, beforeOrAt: 0, after: _, afterOrAt: 0));
            AddAssert("Entry 1 after", () => testIndexes() is (before: _, beforeOrAt: _, after: 1, afterOrAt: _));


            AddLabel("End");
            AddStep("Move to t = 1", () =>
            {
                slider.Current.Value = 1;
            });
            AddAssert("No entries after", () => testIndexes() is (before: _, beforeOrAt: _, after: -1, afterOrAt: -1));
            AddAssert("Entry 11 before", () => testIndexes() is (before: 11, beforeOrAt: 11, after: _, afterOrAt: _));


            AddLabel("At Last Entry");
            AddStep("Move to t = 0.99", () =>
            {
                slider.Current.Value = 0.99;
            });
            AddAssert("No entries after", () => testIndexes() is (before: _, beforeOrAt: _, after: -1, afterOrAt: _));
            AddAssert("At Entry 11", () => testIndexes() is (before: _, beforeOrAt: 11, after: _, afterOrAt: 11));
            AddAssert("Entry 10 before", () => testIndexes() is (before: 10, beforeOrAt: _, after: _, afterOrAt: _));


            AddLabel("Between");
            AddStep("Move to t = 0.6", () =>
            {
                slider.Current.Value = 0.6;
            });
            AddAssert("Entry 6 before", () => testIndexes() is (before: 6, beforeOrAt: 6, after: _, afterOrAt: _));
            AddAssert("Entry 7 after", () => testIndexes() is (before: _, beforeOrAt: _, after: 7, afterOrAt: 7));


            AddLabel("At Entry");
            AddStep("Move to t = 0.75", () =>
            {
                slider.Current.Value = 0.75;
            });
            AddAssert("At entry 7", () => testIndexes() is (before: _, beforeOrAt: 7, after: _, afterOrAt: 7));
            AddAssert("Entry 6 before", () => testIndexes() is (before: 6, beforeOrAt: _, after: _, afterOrAt: _));
            AddAssert("Entry 8 after", () => testIndexes() is (before: _, beforeOrAt: _, after: 8, afterOrAt: _));


            AddLabel("Before Multiple Entries");
            AddStep("Move to t = 0.075", () =>
            {
                slider.Current.Value = 0.075;
            });
            AddAssert("Entry 2 after", () => testIndexes() is (before: _, beforeOrAt: _, after: 2, afterOrAt: _));


            AddLabel("After Multiple Entries");
            AddStep("Move to t = 0.2", () =>
            {
                slider.Current.Value = 0.2;
            });
            AddAssert("Entry 4 before", () => testIndexes() is (before: 4, beforeOrAt: _, after: _, afterOrAt: _));


            AddLabel("At Multiple Entries");
            AddStep("Move to t = 0.1", () =>
            {
                slider.Current.Value = 0.1;
            });
            AddAssert("At entry 2", () => testIndexes() is (before: _, beforeOrAt: 2, after: _, afterOrAt: 2));
            AddAssert("Entry 1 before", () => testIndexes() is (before: 1, beforeOrAt: _, after: _, afterOrAt: _));
            AddAssert("Entry 5 after", () => testIndexes() is (before: _, beforeOrAt: _, after: 5, afterOrAt: _));
        }
    }

    public class TimelineMarker : CompositeDrawable
    {
        public int Order;
        private SpriteText label;
        private string labelText = "";
        public string LabelText
        {
            get => labelText;
            set => label.Text = labelText = value;
        }

        public TimelineMarker()
        {
            RelativePositionAxes = Axes.X;
            Origin = Anchor.BottomCentre;
            Anchor = Anchor.CentreLeft;

            AddInternal(new Circle
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Size = new Vector2(8)
            });

            AddInternal(label = new SpriteText
            {
                Origin = Anchor.TopCentre,
                Anchor = Anchor.BottomCentre,
                Scale = new Vector2(0.8f),

                Y = 10
            });
        }
    }
}
