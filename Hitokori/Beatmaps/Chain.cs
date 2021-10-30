using osu.Game.Rulesets.Hitokori.Objects;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class Chain {
		public Chain ( TilePoint beginning ) {
			Beginning = beginning;
		}

		public int ID {
			get => Beginning.ChainID;
			set {
				foreach ( var i in Beginning.AllInChain ) {
					i.ChainID = value;
				}
			}
		}

		public TilePoint Beginning;
		string? name = null;
		[AllowNull]
		public string Name {
			get => name ?? (ID+1).ToSpreadsheetNotation();
			set => name = value;
		}
	}
}
