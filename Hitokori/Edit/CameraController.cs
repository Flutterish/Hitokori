using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using osuTK;
using osuTK.Input;
using System;

namespace osu.Game.Rulesets.Hitokori.Edit {
	public class CameraController : CompositeDrawable {
		private HitokoriHitObjectComposer composer;

		public CameraController ( HitokoriHitObjectComposer composer ) {
			this.composer = composer;
			RelativeSizeAxes = Axes.Both;
		}

		public override bool DragBlocksClick => true;

		protected override bool OnScroll ( ScrollEvent e ) {
			if ( composer.ManualCameraToggle.Value == TernaryState.True ) {
				var initialScale = composer.Playfield.CameraScale.Value;
				var zoom = Math.Log2( initialScale );
				zoom += e.ScrollDelta.Y / 5;
				zoom = Math.Clamp( zoom, 2, 12 );
				var endScale = Math.Pow( 2, zoom );


				var center = composer.Playfield.CameraMiddle.Value;
				var target = composer.Playfield.NormalizedPositionAtScreenSpace( e.ScreenSpaceMousePosition );
				var delta = target - center;
				var newDelta = target + delta * (float)( endScale / initialScale - 1 ) - center;
				delta = newDelta - delta;

				composer.Playfield.CameraScale.Value = endScale;
				composer.Playfield.CameraMiddle.Value += delta;

				return true;
			}
			else return false;
		}

		bool isDragged = false;
		protected override bool OnMouseDown ( MouseDownEvent e ) {
			if ( composer.ManualCameraToggle.Value == TernaryState.True ) {
				if ( e.Button is MouseButton.Right or MouseButton.Middle ) {
					isDragged = true;
					return true;
				}
				else return false;
			}
			else return false;
		}

		protected override void OnMouseUp ( MouseUpEvent e ) {
			isDragged = false;
			base.OnMouseUp( e );
		}

		protected override bool OnMouseMove ( MouseMoveEvent e ) {
			if ( composer.ManualCameraToggle.Value != TernaryState.True || !isDragged )
				return false;

			var unit = composer.Playfield.ScreenSpacePositionOf( Vector2.One ) - composer.Playfield.ScreenSpacePositionOf( Vector2.Zero );
			var delta = ToScreenSpace( e.Delta ) - ToScreenSpace( Vector2.Zero );

			composer.Playfield.CameraMiddle.Value -= Vector2.Divide( delta, unit );

			return true;
		}
	}
}
