using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Hitokori.Objects;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors {
	public class RotationConnectorBlueprint : ConnectorBlueprint<TilePointRotationConnector> {
		const float size = 120;
		const float borderSize = 10;

		public RotationConnectorBlueprint ( TilePointRotationConnector connector ) : base( connector ) {
			AddInternal( angleDelta = new HitboxedCircularProgress {
				Origin = Anchor.Centre,
				Size = new Vector2( size ),
				Colour = Colour4.Yellow,
				Alpha = 0.4f
			} );
			AddInternal( radiusHandle = new DistanceHandle {
				Origin = Anchor.Centre,
				Size = new Vector2( size ),
				Colour = Colour4.Yellow,
				InnerRadius = borderSize / size,
				Current = angleDelta.Current,
				Dragged = onDistanceHandleDragged
			} );
			AddInternal( angleHandle = new RotationHandle {
				Origin = Anchor.Centre,
				Size = new Vector2( borderSize * 2 ),
				Colour = Colour4.Yellow,
				Dragged = onRotationHandleDragged
			} );
		}

		private CircularProgress angleDelta;
		private DistanceHandle radiusHandle;
		private RotationHandle angleHandle;

		[BackgroundDependencyLoader]
		private void load ( OsuColour colours ) {
			angleDelta.Colour = colours.YellowDark;
			radiusHandle.Colour = colours.YellowDark;
			angleHandle.Colour = colours.YellowDark;
		}

		private void onRotationHandleDragged ( DragEvent e ) {
			var b = Connector.GetPositionAt( 1 ) - Connector.GetStateAt( 1 ).StackingOffsetOfNth( Connector.TargetOrbitalIndex );
			var pivot = Connector.GetStateAt( 0 ).PivotPosition;

			var startAngle = pivot.AngleTo( b );
			var endAngle = PositionAt( pivot ).AngleTo( e.MousePosition );
			var delta = startAngle.AngleDifference( endAngle );

			if ( e.CurrentState.Keyboard.ControlPressed ) {
				var snap = e.CurrentState.Keyboard.ShiftPressed ? Math.Tau / 36 /* 10 deg */  : Math.Tau / 16 /* 22.5 deg */ ;
				Connector.Angle.ConstrainRadians( Math.Round( ( Connector.Angle.ValueRadians + delta ) / snap ) * snap );
			}
			else {
				Connector.Angle.ConstrainRadians( Connector.Angle.ValueRadians + delta );
			}

			InvokeModified();
		}

		private void onDistanceHandleDragged ( DragEvent e ) {
			var pivot = Connector.GetStateAt( 0 ).PivotPosition;
			var pos = Playfield.NormalizedPositionAtScreenSpace( e.ScreenSpaceMousePosition );

			var distance = ( pos - (Vector2)pivot ).Length;
			if ( e.CurrentState.Keyboard.ControlPressed ) {
				var snap = e.CurrentState.Keyboard.ShiftPressed ? 0.1 : 0.5;
				Connector.Radius.Constrain( Math.Round( distance / snap ) * snap );
			}
			else {
				Connector.Radius.Constrain( distance );
			}

			InvokeModified();
		}

		protected override void Update () {
			base.Update();

			var pivot = Connector.GetStateAt( 0 ).PivotPosition;
			var a = Connector.GetPositionAt( 0 ) - Connector.GetStateAt( 0 ).StackingOffsetOfNth( Connector.TargetOrbitalIndex );
			var b = Connector.GetPositionAt( 1 ) - Connector.GetStateAt( 1 ).StackingOffsetOfNth( Connector.TargetOrbitalIndex );

			var size = ( PositionAt( pivot ) - PositionAt( b ) ).Length * 2;
			radiusHandle.Size = angleDelta.Size = new Vector2( size );
			radiusHandle.InnerRadius = borderSize / size;

			radiusHandle.Position = angleDelta.Position = PositionAt( pivot );
			angleHandle.Position = angleDelta.Position + (Vector2)pivot.AngleTo( b ).AngleToVector( size / 2 );

			if ( Connector.Angle.Value < 0 ) {
				angleDelta.Current.Value = Connector.Angle.ValueDegrees / 360;
				radiusHandle.Rotation = angleDelta.Rotation = (float)pivot.AngleTo( b ).RadToDeg() + 90;
			}
			else {
				angleDelta.Current.Value = Connector.Angle.ValueDegrees / 360;
				radiusHandle.Rotation = angleDelta.Rotation = (float)pivot.AngleTo( a ).RadToDeg() + 90;
			}
		}

		public override bool CanResetConstraints => Connector.Angle.IsConstrained || Connector.Radius.IsConstrained || Connector.Velocity.IsConstrained;
		public override void ResetConstraints () {
			Connector.Angle.ReleaseConstraint();
			Connector.Radius.ReleaseConstraint();
			Connector.Velocity.ReleaseConstraint();
		}

		private class RotationHandle : CompositeDrawable {
			public Action<DragEvent>? Dragged;

			public RotationHandle () {
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

		private class HitboxedCircularProgress : CircularProgress {
			public override bool ReceivePositionalInputAt ( Vector2 screenSpacePos ) {
				var pos = ToLocalSpace( screenSpacePos ) - DrawSize / 2;
				var radius = pos.Length;
				var maxRadius = DrawSize.X / 2;

				var delta = Math.Abs( Current.Value * Math.Tau );

				var angle = Vector2.Zero.AngleTo( pos ) + MathF.PI / 2;

				return radius <= maxRadius && Math.Abs( angle.AngleDifference( (float)delta / 2 ) ) <= delta / 2;
			}
		}

		private class DistanceHandle : HitboxedCircularProgress {
			public Action<DragEvent>? Dragged;

			protected override bool OnDragStart ( DragStartEvent e ) {
				return true;
			}

			protected override void OnDrag ( DragEvent e ) {
				Dragged?.Invoke( e );
			}
		}
	}
}
