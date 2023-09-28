using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.UI {
    public class HitokoriIcon : Container
    {
		[Resolved]
		private LocalisationManager localisation { get; set; }

		public HitokoriIcon () {
			this.Center();
			RelativeSizeAxes = Axes.Both;

			Children = new Drawable[] {
				new SpriteIcon {
					Icon = FontAwesome.Regular.Circle,
					RelativeSizeAxes = Axes.Both
				}.Center(),
				new SpriteIcon {
					Icon = FontAwesome.Solid.Fire,
					RelativeSizeAxes = Axes.Both,
					Scale = new Vector2( 0.85f )
				}.Center()
			};
		}
	}
}
