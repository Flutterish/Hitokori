using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Utils;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class TargetMarker : CompositeDrawable {
		public readonly Bindable<Colour4> accentColour = new();
		public TargetMarker () {
			InternalChild = new SpriteIcon { Icon = FontAwesome.Regular.Circle, Alpha = 0, AlwaysPresent = true }.Center();
		}

		public void Appear ( TickSize size ) {
			lastAnimation = () => Appear( size );
			lastAnimationTime = TransformStartTime;

			InternalChild.Colour = accentColour.Value;
			InternalChild.ResizeTo( (float)( size.Size() * 2 + 8 ) )
				.FadeInFromZero( 100 ).ResizeTo( (float)( size.Size() + 8 ), 200 );
		}

		public void Disappear () {
			lastAnimation = Disappear;
			lastAnimationTime = TransformStartTime;

			InternalChild.FadeOut( 100 );
		}

		public void PlayLastAnimation () {
			if ( lastAnimation != null ) {
				using ( BeginAbsoluteSequence( lastAnimationTime, true ) ) {
					lastAnimation();
				}
			}
		}

		Action lastAnimation;
		double lastAnimationTime;

		[BackgroundDependencyLoader]
		private void load ( Bindable<Colour4> accent ) {
			accentColour.BindTo( accent );
			accentColour.BindValueChanged( v => InternalChild.FadeColour( v.NewValue, 100 ), true );
		}
	}
}
