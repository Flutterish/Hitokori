using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays.Settings;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class ColorPicker<T> : SettingsItem<Color4> where T : ColorPickerControl, new() {
		public new ColorPickerControl Control => (ColorPickerControl)base.Control;
		protected override Drawable CreateControl () {
			return new T();
		}

		public ColorPicker () {
			FlowContent.Direction = FillDirection.Vertical;
		}
	}

	public class ColorPicker : ColorPicker<ColorPickerControl> { }
}
