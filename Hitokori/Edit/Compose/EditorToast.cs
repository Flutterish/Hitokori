using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose {
	public class EditorToast : VisibilityContainer {
		OsuSpriteText text;
		Bindable<TernaryState> showTooltipsToggle = new Bindable<TernaryState>( TernaryState.True );

		public EditorToast ( Bindable<TernaryState> showTooltipsToggle ) {
			Origin = Anchor.Centre;
			Anchor = Anchor.BottomCentre;
			AutoSizeAxes = Axes.Both;
			Padding = new MarginPadding( 6 );
			AddInternal( text = new OsuSpriteText() );
			Y = -5;

			this.showTooltipsToggle.BindTo( showTooltipsToggle );

			showTooltipsToggle.BindValueChanged( v => {
				if ( v.NewValue == TernaryState.False )
					Hide();
				else
					ShowMessage( "Tooltips will show right here!" );
			} );
		}

		private double toastStartTime;
		public void ShowMessage ( LocalisableString message ) {
			if ( showTooltipsToggle.Value != TernaryState.True )
				return;

			text.Text = message;
			toastStartTime = Time.Current;
			Show();
		}

		protected override void Update () {
			base.Update();

			if ( toastStartTime + 3000 < Time.Current )
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
