using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
    public class Hi : StandardOrbital
    {
		public Hi ( IHasTilePosition parent, Radius radius ) : base( parent, radius, Color4.Red ) { }

		Bindable<Color4> color = new( Color4.Red );
		[BackgroundDependencyLoader(permitNulls: true)]
		private void load ( HitokoriSettingsManager config ) {
			config?.BindWith( HitokoriSetting.HiColor, color );
			color.BindValueChanged( v => {
				Colour = v.NewValue;
			}, true );
		}
	}
}
