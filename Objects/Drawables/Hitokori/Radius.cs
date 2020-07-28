using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class Radius : CircularContainer {
		public Radius () {
			BorderColour = Color4.White;
			BorderThickness = 2;
			Masking = true;
			Width = 0;
			Height = 0;
			Alpha = 0.6f;
			InternalChild = new Box {
				RelativeSizeAxes = Axes.Both,
				AlwaysPresent = true,
				Alpha = 0
			}.Center();
		}

		public void AnimateDistance ( double length, double duration, Easing easing = Easing.None ) {
			this.ResizeTo( (float)length * 2, duration, easing );
		}

		public double Length => Width / 2;
	}
}
