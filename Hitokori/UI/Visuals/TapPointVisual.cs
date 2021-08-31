using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.UI.Visuals {
	public class TapPointVisual : AppliableVisual<PassThroughTilePoint> {
		public TapPointVisual () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AddInternal( new Circle {
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 22 ),
				Colour = Colour4.White
			} );
			AddInternal( new Circle {
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 18 ),
				Colour = Colour4.HotPink
			} );
		}
	}
}
