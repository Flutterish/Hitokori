using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using System;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors {
	public class MoveHandle : CompositeDrawable {
		public Action<DragEvent>? Dragged;

		public MoveHandle () {
			AddInternal( new Circle {
				RelativeSizeAxes = Axes.Both
			} );
			AddInternal( new HoverSounds {
				RelativeSizeAxes = Axes.Both
			} );
		}

		private void prompt () {
			this.ScaleTo( 1.4f, 100 );
		}
		private void unprompt () {
			this.ScaleTo( 1f, 100 );
		}

		protected override bool OnDragStart ( DragStartEvent e ) {
			if ( !IsHovered ) prompt();
			return true;
		}

		protected override void OnDrag ( DragEvent e ) {
			Dragged?.Invoke( e );
		}

		protected override void OnDragEnd ( DragEndEvent e ) {
			if ( !IsHovered ) unprompt();
			base.OnDragEnd( e );
		}

		protected override bool OnHover ( HoverEvent e ) {
			if ( !IsDragged ) prompt();
			return true;
		}

		protected override void OnHoverLost ( HoverLostEvent e ) {
			if ( !IsDragged ) unprompt();
			base.OnHoverLost( e );
		}
	}
}
