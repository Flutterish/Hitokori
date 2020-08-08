﻿using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class Kori : StandardOrbital {
		public Kori ( IHasTilePosition parent, Radius radius ) : base( parent, radius, Color4.Blue ) { }
	}
}
