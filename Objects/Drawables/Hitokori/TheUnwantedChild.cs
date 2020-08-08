using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class TheUnwantedChild : StandardOrbital {
		public TheUnwantedChild ( IHasTilePosition parent, Radius radius ) : base( parent, radius, Color4.Green ) { }
	}
}
