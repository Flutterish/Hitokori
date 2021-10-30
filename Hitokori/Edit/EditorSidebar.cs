using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays;
using osuTK;
using osuTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.Hitokori.Edit {
	public class EditorSidebar : VisibilityContainer {
		public EditorSidebar () {
			Origin = Anchor.Centre;
			Anchor = Anchor.CentreRight;

			RelativeSizeAxes = Axes.Y;
			Width = 500;

			X = -250;

			Masking = true;
			CornerRadius = 8;
		}

		protected override bool StartHidden => true;

		[BackgroundDependencyLoader]
		private void load () {
			AddInternal( new Box {
				RelativeSizeAxes = Axes.Both,
				Colour = OsuColour.Gray( 0.1f ),
				Alpha = 0.8f
			} );

			OsuScrollContainer scroll;
			AddInternal( scroll = new OsuScrollContainer( Direction.Vertical ) {
				Position = new Vector2( 5 ),
				Child = content = new FillFlowContainer {
					Direction = FillDirection.Vertical,
					RelativeSizeAxes = Axes.X,
					AutoSizeAxes = Axes.Y
				}
			} );
			scroll.OnUpdate += d => d.Size = new Vector2( Width - 10, DrawHeight - 10 );
		}

		[MaybeNull, NotNull]
		FillFlowContainer content;
		protected override Container<Drawable> Content => content;

		public override bool ReceivePositionalInputAt ( Vector2 screenSpacePos )
			=> true;

		protected override bool OnClick ( ClickEvent e ) {
			if ( e.Button is MouseButton.Left && !base.ReceivePositionalInputAt( e.ScreenSpaceMousePosition ) )
				Hide();

			return false;
		}

		protected override void PopIn () {
			this.FadeIn( 200 );
			this.ScaleTo( 1, 300, Easing.OutBack );
		}

		protected override void PopOut () {
			this.FadeOut( 200 );
			this.ScaleTo( 0.7f, 300, Easing.In );
		}
	}
}
