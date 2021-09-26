using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Hitokori.Tests.TestingUI {
	public class DragablePoint : CompositeDrawable {
		private BindableWithCurrent<Vector2> current = new();

		public Bindable<Vector2> Current {
			get => current;
			set => current.Current = value;
		}

		public Vector2 Value {
			get => current.Value;
			set => current.Value = value;
		}

		private bool isDragable = true;
		public bool IsDragable {
			get => isDragable;
			set {
				isDragable = value;
				point.BorderThickness = value ? 0 : 3;
			}
		}

		Circle point;
		public DragablePoint () {
			AutoSizeAxes = Axes.Both;
			Origin = Anchor.Centre;

			AddInternal( point = new Circle {
				Origin = Anchor.Centre,
				Anchor = Anchor.Centre,
				Size = new Vector2( 20 ),
				Colour = Colour4.Blue,
				BorderColour = Colour4.White,
				Alpha = 0.8f
			} );

			Current.BindValueChanged( v => {
				Position = v.NewValue;
			} );
		}

		protected override void Update () {
			base.Update();

			isActive = IsDragged || IsHovered;
		}

		bool wasActive;
		bool isActive {
			get => wasActive;
			set {
				if ( wasActive == value ) return;

				point.ScaleTo( value ? 1.25f : 1, 100 );
				wasActive = value;
			}
		}

		protected override bool OnDragStart ( DragStartEvent e ) {
			return isDragable && e.Button == MouseButton.Left;
		}
		protected override void OnDrag ( DragEvent e ) {
			current.Value = e.MousePosition;
		}
	}
}
