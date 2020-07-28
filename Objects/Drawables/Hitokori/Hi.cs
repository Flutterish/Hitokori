using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class Hi : Orbital {
		public Hi ( IHasTilePosition parent ) : base( parent ) {
			AddInternal( 
				new Circle {
					Width = HitokoriTile.SIZE * 2,
					Height = HitokoriTile.SIZE * 2,
					Colour = Color4.Red
				}.Center()
			);

			Trail.Colour = Color4.Red;
		}
	}
}
