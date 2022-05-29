using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Connections;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors {
	public class JumpingLinearConnectorBlueprint : LinearConnectorBlueprint {
		new public JumpingTilePointLinearConnector Connector => (JumpingTilePointLinearConnector)base.Connector;

		MoveHandle handle;
		public JumpingLinearConnectorBlueprint ( JumpingTilePointLinearConnector connector ) : base( connector ) {
			Add( handle = new MoveHandle {
				Size = new Vector2( 20 ),
				Origin = Anchor.Centre,
				Dragged = onHandleMoved
			} );
		}

		[BackgroundDependencyLoader]
		private void load ( OsuColour colours ) {
			handle.Colour = colours.YellowDark;
		}

		private void onHandleMoved ( DragEvent e ) {
			if ( IsDisposed ) return;

			var pos = Connector.GetPositionAt( 0.5 );
			var delta = ((Vector2)pos - Playfield.NormalizedPositionAtScreenSpace( e.ScreenSpaceMousePosition )).Y;

			Connector.JumpZ = ( Connector.Distance == 0 ) ? 0 : (delta / Connector.Distance);
		}

		protected override void Update () {
			if ( IsDisposed ) return;
			base.Update();

			handle.Position = PositionAt( Connector.GetPositionAt( 0.5 ) - Vector2d.UnitY * Connector.Distance * Connector.JumpZ );
		}
	}
}
