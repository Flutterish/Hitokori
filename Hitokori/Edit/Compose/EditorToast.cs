using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose {
	public class EditorToast : VisibilityContainer {
		OsuSpriteText text;
		Bindable<TernaryState> showTooltipsToggle = new Bindable<TernaryState>( TernaryState.True );

		public EditorToast ( Bindable<TernaryState> showTooltipsToggle ) {
			Origin = Anchor.Centre;
			Anchor = Anchor.BottomCentre;
			AutoSizeAxes = Axes.Both;
			Padding = new MarginPadding( 6 );
			AddInternal( new FillFlowContainer {
				Direction = FillDirection.Horizontal,
				AutoSizeAxes = Axes.Both,
				Children = new Drawable[] {
					new SpriteIcon {
						Size = new Vector2( 16 ),
						Icon = FontAwesome.Solid.InfoCircle,
						Margin = new MarginPadding { Right = 4 }
					},
					text = new OsuSpriteText()
				}
			} );
			Y = -5;

			this.showTooltipsToggle.BindTo( showTooltipsToggle );

			showTooltipsToggle.BindValueChanged( v => {
				if ( v.NewValue == TernaryState.False ) {
					lastMessage = "";
					Hide();
				}
				else
					ShowMessage( "Tooltips will show right here!", 4000 );
			} );
		}

		private double toastStartTime;
		private double toastDuration;
		private LocalisableString lastMessage;
		public void ShowMessage ( LocalisableString message, double duration = double.PositiveInfinity ) {
			if ( lastMessage == message )
				return;

			if ( showTooltipsToggle.Value != TernaryState.True )
				return;

			text.Text = lastMessage = message;
			toastStartTime = Time.Current;
			toastDuration = duration;
			Show();
		}

		public void HideMessage ( LocalisableString message ) {
			if ( lastMessage == message ) {
				lastMessage = "";
				Hide();
			}
		}

		protected override void Update () {
			base.Update();

			if ( toastStartTime + toastDuration < Time.Current )
				Hide();
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
