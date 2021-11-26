using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.UI.Visuals
{
    public class TapPointVisualPiece : CompositeDrawable
    {
        protected Drawable Body;
        protected Drawable BodyOutline;
        public readonly SpriteIcon Icon;

        public TapPointVisualPiece()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AutoSizeAxes = Axes.Both;

            AddInternal(BodyOutline = new Circle
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(22),
                Colour = Colour4.White
            });
            AddInternal(Body = new Circle
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(18),
                Colour = Colour4.HotPink
            });
            AddInternal(Icon = new SpriteIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(14),
                Alpha = 0
            });
        }

        public void RestoreDefaults()
        {
            Colour = Colour4.HotPink;
            BorderColour = Colour4.White;
            Icon.Alpha = 0;
            Icon.Colour = Colour4.White;
        }

        new public Color4 Colour
        {
            get => Body.Colour;
            set
            {
                Body.Colour = value;
            }
        }

        new public Color4 BorderColour
        {
            get => BodyOutline.Colour;
            set
            {
                BodyOutline.Colour = value;
            }
        }
    }
}
