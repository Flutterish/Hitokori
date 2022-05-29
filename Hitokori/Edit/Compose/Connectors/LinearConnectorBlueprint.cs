﻿using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Connections;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors {
	public class LinearConnectorBlueprint : ConnectorBlueprint<TilePointLinearConnector> {
		MoveHandle handle;
		public LinearConnectorBlueprint ( TilePointLinearConnector connector ) : base( connector ) {
			Add( handle = new MoveHandle {
				Size = new Vector2( 20 ),
				Origin = Anchor.Centre,
				Dragged = onHandleMoved,
				DragEnded = _ => Tooltip = string.Empty
			} );
		}

		[BackgroundDependencyLoader]
		private void load ( OsuColour colours ) {
			handle.Colour = colours.YellowDark;
		}

		private void onHandleMoved ( DragEvent e ) {
			if ( IsDisposed ) return;

			var posFrom = Connector.GetPositionAt( 0 );
			var posTo = Playfield.NormalizedPositionAtScreenSpace( e.ScreenSpaceMousePosition );

			var angle = posFrom.AngleTo( (Vector2d)posTo );
			var length = ( posTo - (Vector2)posFrom ).Length;

			if ( e.CurrentState.Keyboard.ControlPressed ) {
				Tooltip = e.CurrentState.Keyboard.ShiftPressed
					? "[Ctrl] + [Shift] → 0.1 Tiles, 10°"
					: "[Ctrl] + Shift → 0.5 Tiles, 22.5°";

				var angleSnap = e.CurrentState.Keyboard.ShiftPressed ? ( Math.Tau / 36 /* 10 deg */) : ( Math.Tau / 16 /* 22.5 deg */);
				var distanceSnap = e.CurrentState.Keyboard.ShiftPressed ? 0.1 : 0.5;

				Connector.Angle = Math.Round( angle / angleSnap ) * angleSnap;
				Connector.Distance.Constrain( Math.Round( length / distanceSnap ) * distanceSnap );
			}
			else {
				Tooltip = "Ctrl and Ctrl + Shift to snap";
				Connector.Angle = angle;
				Connector.Distance.Constrain( length );
			}

			InvokeModified();
		}

		public override bool CanResetConstraints => Connector.Distance.IsConstrained || Connector.Velocity.IsConstrained;
		public override void ResetConstraints () {
			Connector.Distance.ReleaseConstraint();
			Connector.Velocity.ReleaseConstraint();
		}

		protected override void Update () {
			if ( IsDisposed ) return;
			base.Update();

			handle.Position = PositionAt( Connector.GetEndPosition() );
		}
	}
}
