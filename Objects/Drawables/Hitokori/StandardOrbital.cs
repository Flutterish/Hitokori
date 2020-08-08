using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class StandardOrbital : Orbital {
		private Circle circle;
		public StandardOrbital ( IHasTilePosition parent, Radius radius, Colour4 colour ) : base( parent, radius ) {
			AddInternal(
				circle = new Circle {
					Width = HitokoriTile.SIZE * 2,
					Height = HitokoriTile.SIZE * 2,
					Colour = colour
				}.Center()
			);

			Trail.Colour = colour;

			RevokeImportant();
		}

		public override void MakeImportant () {
			circle.ScaleTo( 1.1f, 100 );
			circle.FadeTo( 1, 100 );
		}

		public override void RevokeImportant () {
			circle.ScaleTo( 0.9f, 100 );
			circle.FadeTo( 0.8f, 100 );
		}
	}
}
