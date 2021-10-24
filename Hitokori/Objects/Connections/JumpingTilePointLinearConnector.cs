using osu.Game.Rulesets.Hitokori.Orbitals;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Connections {
	public class JumpingTilePointLinearConnector : TilePointLinearConnector {
		public override OrbitalState GetStateAt ( double progress ) {
			var k = Math.Clamp( progress, 0, 1 ) * 2 - 1;
			return base.GetStateAt( progress ).WithZ( ( 1 - k * k ) * Distance * 0.3 );
		}
	}
}
