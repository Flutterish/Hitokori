using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class ImportantMarker : Container { // BUG important marker can overlap reverse marker
		TickSize TickSize;
		public ImportantMarker ( TickSize size ) {
			TickSize = size;

			for ( int i = 0; i < 3; i++ ) {
				var pointer = new SpriteIcon {
					Icon = FontAwesome.Solid.ArrowDown,
					Width = 20,
					Height = 20,
					Colour = Colour4.Yellow
				}.Center();

				pointer.Rotation = 360 / 3 * i + 90;
				var angle = Math.PI * 2 / 3 * i;
				pointer.Position = new Vector2( (float)Math.Cos( angle ), (float)Math.Sin( angle ) ) * (float)size.Size() * 1.3f;

				AddInternal( pointer );
			}
		}

		public void Spin () {
			this.Spin( 4000, RotationDirection.Clockwise ).Loop();
		}

		public void Disappear () {
			this.FadeOut( 500 );
		}

		public void Appear () {
			this.FadeInFromZero( 500 );
		}
	}
}
