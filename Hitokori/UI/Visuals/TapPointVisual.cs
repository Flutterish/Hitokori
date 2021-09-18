using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.UI.Visuals {
	public class TapPointVisual : AppliableVisual<PassThroughTilePoint> {
		Drawable body;
		Drawable bodyOutline;

		public TapPointVisual () {
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;

			AddInternal( bodyOutline = new Circle {
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 22 ),
				Colour = Colour4.White
			} );
			AddInternal( body = new Circle {
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Size = new Vector2( 18 ),
				Colour = Colour4.HotPink
			} );
		}

		new public Color4 Colour {
			get => body.Colour;
			set {
				body.Colour = value;
			}
		}

		protected override void OnApply ( PassThroughTilePoint hitObject ) {
			base.OnApply( hitObject );

			if ( hitObject.FromPrevious is IHasVelocity fromv && hitObject.ToNext is IHasVelocity tov ) {
				if ( fromv.Speed / tov.Speed < 0.95 ) {
					Colour = Colour4.Tomato;
				}
				else if ( tov.Speed / fromv.Speed < 0.95 ) {
					Colour = Colour4.LightBlue;
				}
				else {
					Colour = Colour4.HotPink;
				}
			}
		}
	}
}
