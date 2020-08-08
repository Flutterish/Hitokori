using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class TheUnwantedChild : Orbital {
		public TheUnwantedChild ( IHasTilePosition parent, Radius radius ) : base( parent, radius ) {
			AddInternal(
				new Circle {
					Width = HitokoriTile.SIZE * 2,
					Height = HitokoriTile.SIZE * 2,
					Colour = Color4.Green
				}.Center()
			);

			Trail.Colour = Color4.Green;
		}
	}
}
