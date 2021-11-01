using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays;
using osuTK;
using osuTK.Input;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose {
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
		private void load ( OverlayColourProvider colourProvider ) {
			AddInternal( new Box {
				RelativeSizeAxes = Axes.Both,
				Colour = colourProvider.Background6,
				Alpha = 0.6f
			} );

			AddInternal( new OsuScrollContainer( Direction.Vertical ) {
				ScrollbarVisible = false,
				Padding = new MarginPadding( 14 ),
				RelativeSizeAxes = Axes.Both,
				Child = content = new FillFlowContainer {
					Direction = FillDirection.Vertical,
					RelativeSizeAxes = Axes.X,
					AutoSizeAxes = Axes.Y
				}
			} );
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
