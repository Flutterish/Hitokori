using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class TheUnwantedChild : StandardOrbital {
		public TheUnwantedChild ( IHasTilePosition parent, Radius radius ) : base( parent, radius, Color4.Green ) { }

		Bindable<Color4> color = new( Color4.Green );
		[BackgroundDependencyLoader(permitNulls: true)]
		private void load ( HitokoriSettingsManager config ) {
			config?.BindWith( HitokoriSetting.GreenBitchColor, color );
			color.BindValueChanged( v => {
				Colour = v.NewValue;
			}, true );
		}
	}
}
