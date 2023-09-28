using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
    public class ReverseMarker : Container
    { // TODO dear princess celestia, why the fuck did i use containers instead of composite drawables?
        bool ToClockwise;
		public ReverseMarker () {
			InternalChild = new SpriteIcon {
				Icon = FontAwesome.Solid.RedoAlt,
				Width = HitokoriTile.SIZE * 2,
				Height = HitokoriTile.SIZE * 2,
				Colour = Colour4.Yellow
			}.Center();
		}

		public void SetClockwise ( bool toClockwise = true ) {
			if ( !( ToClockwise = toClockwise ) ) InternalChild.Scale = new osuTK.Vector2( -1, 1 );
			else InternalChild.Scale = new osuTK.Vector2( 1, 1 );
		}

		public void Spin () {
			Easing easing = Easing.InOutCirc;
			double duration = 500;

			InternalChild.RotateTo( 0 )
				.Then().RotateTo( ( ToClockwise ? 180 : -180 ), duration, easing )
				.Then().RotateTo( ( ToClockwise ? 360 : -360 ), duration, easing )
				.Loop();
		}

		public double Appear () {
			Scale = Vector2.One;
			InternalChild.FadeInFromZero( 500 );

			return 500;
		}

		public double Disappear () {
			InternalChild.FadeOutFromOne( 500 );
			this.ScaleTo( Scale * 1.4f, 300 );

			return 500;
		}
	}
}
