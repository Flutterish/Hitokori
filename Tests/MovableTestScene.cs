using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Events;
using osu.Framework.Testing;
using System;

#nullable enable

namespace osu.Game.Rulesets.Hitokori.Tests
{
    public abstract class MovableTestScene : TestScene
    {
        private Container container;

        public MovableTestScene()
        {
            base.Add(container = new PersistentContainer
            {
                RelativeSizeAxes = Axes.Both,
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre
            });
        }

        public override void Add(Drawable drawable)
        {
            container.Add(drawable);
        }
        public override bool Remove(Drawable drawable)
        {
            return container.Remove(drawable);
        }

        public void AddOverlay(Drawable drawable)
        {
            base.Add(drawable);
        }
        public bool RemoveOverlay(Drawable drawable)
        {
            return base.Remove(drawable);
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            return e.Button == osuTK.Input.MouseButton.Right;
        }

        protected override void OnDrag(DragEvent e)
        {
            container.Position += e.Delta;
        }

        private double zoom = 0;
        protected override bool OnScroll(ScrollEvent e)
        {
            var oldScale = MathF.Pow(2, (float)zoom);
            var oldOffset = container.Position;
            zoom += e.ScrollDelta.Y / 5;
            var newScale = MathF.Pow(2, (float)zoom);
            var newOffset = container.Position * (newScale / oldScale);

            container.Scale = new osuTK.Vector2(newScale);
            container.Position += newOffset - oldOffset;

            return true;
        }

        private class PersistentContainer : Container
        {
            protected override bool ComputeIsMaskedAway(RectangleF maskingBounds) => false;
        }
    }
}
