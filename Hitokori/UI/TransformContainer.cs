using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.UI
{
    public class TransformContainer<T> : Container<T> where T : Drawable
    {
        public TransformContainer()
        {
            AutoSizeAxes = Axes.None;
            Size = Vector2.Zero;
        }

        protected override bool ComputeIsMaskedAway(RectangleF maskingBounds)
            => false;
    }

    public class TransformContainer : TransformContainer<Drawable>
    {

    }
}
