using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.UI.Visuals {
	public class TapPointVisualPiece : CompositeDrawable {
		protected Drawable Body;
		protected Drawable BodyOutline;

		public TapPointVisualPiece () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AddInternal( BodyOutline = new Circle {
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 22 ),
				Colour = Colour4.White
			} );
			AddInternal( Body = new Circle {
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 18 ),
				Colour = Colour4.HotPink
			} );
		}

		new public Color4 Colour {
			get => Body.Colour;
			set {
				Body.Colour = value;
			}
		}

		new public Color4 BorderColour {
			get => Body.Colour;
			set {
				BodyOutline.Colour = value;
			}
		}
	}
}
