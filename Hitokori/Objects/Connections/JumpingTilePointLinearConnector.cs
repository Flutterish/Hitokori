using osu.Game.Rulesets.Hitokori.Edit.Connectors;
using osu.Game.Rulesets.Hitokori.Orbitals;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Connections {
	public class JumpingTilePointLinearConnector : TilePointLinearConnector {
		[Inspectable( Section = InspectableAttribute.SectionProperties, Label = "Jump height", FormatMethod = nameof( FormatMultiplier ), ParseMethod = nameof( ParseMultiplier ) )]
		public double JumpZ = 0.3;

		public override OrbitalState GetStateAt ( double progress ) {
			var k = Math.Clamp( progress, 0, 1 ) * 2 - 1;
			return base.GetStateAt( progress ).WithZ( ( 1 - k * k ) * Distance * JumpZ );
		}

		public override ConnectorBlueprint CreateEditorBlueprint ()
			=> new JumpingLinearConnectorBlueprint( this );
	}
}
