using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class OrbitalColorPicker : ColorPickerControl {
		StandardOrbital orbital;
		Radius radius;
		protected override Drawable CreatePreview () {
			var r = new Container {
				RelativeSizeAxes = Axes.Both,
				Children = new Drawable[] {
					radius = new Radius {
						RelativeSizeAxes = Axes.Both,
						Size = new Vector2( 0.64f )
					}.Center(),

					orbital = new StandardOrbital( new ZeroTile(), radius, Colour4.White ).Center()
				}
			}.Center();

			orbital.OnLoadComplete += _ => {
				orbital.Velocity = 2.3f / 1000;
				orbital.Release();
			};

			current.BindValueChanged( v => {
				orbital.Colour = v.NewValue;
			}, true );

			return r;
		}

		private class ZeroTile : IHasTilePosition {
			public Vector2 TilePosition => Vector2.Zero;
		}
	}
}
