using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays;
using osu.Game.Screens.Edit.Setup;
using osuTK;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Edit.Setup {
	public abstract class SetupSubsection : SetupSection {
		[MaybeNull, NotNull]
		Drawable side;

		[BackgroundDependencyLoader]
		private void load ( OverlayColourProvider colourProvider ) {
			Padding = Padding with { Horizontal = 20, Top = 5 };
			AddInternal( side = new Circle {
				Colour = colourProvider.Highlight1,
				RelativeSizeAxes = Axes.Y,
				Width = 5,
				Position = new Vector2( -20, 0 ),
			} );

			ShowSideBindable.BindValueChanged( v => {
				side.FadeTo( v.NewValue ? 1 : 0, 200 );
			}, true );
			side.FinishTransforms();
		}

		public BindableBool ShowSideBindable = new( true );
		public bool ShowSide {
			get => ShowSideBindable.Value;
			set => ShowSideBindable.Value = value;
		}
	}
}
