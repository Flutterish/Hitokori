using osu.Game.Rulesets.Hitokori.Objects.Connections;

namespace osu.Game.Rulesets.Hitokori.Edit.Connectors {
	public class JumpingLinearConnectorBlueprint : LinearConnectorBlueprint {
		new public JumpingTilePointLinearConnector Connector => (JumpingTilePointLinearConnector)base.Connector;

		public JumpingLinearConnectorBlueprint ( JumpingTilePointLinearConnector connector ) : base( connector ) {

		}
	}
}
