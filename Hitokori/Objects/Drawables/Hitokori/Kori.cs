using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class Kori : StandardOrbital {
		public Kori ( IHasTilePosition parent, Radius radius ) : base( parent, radius, Color4.Blue ) { }

		Bindable<Color4> color = new( Color4.Blue );
		[BackgroundDependencyLoader(permitNulls: true)]
		private void load ( HitokoriSettingsManager config ) {
			config?.BindWith( HitokoriSetting.KoriColor, color );
			color.BindValueChanged( v => {
				Colour = v.NewValue;
			}, true );
		}
	}
}
