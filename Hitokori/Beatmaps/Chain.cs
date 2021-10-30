using osu.Framework.Bindables;
using osu.Game.Rulesets.Hitokori.Objects;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class Chain {
		public Chain ( TilePoint beginning, string name ) {
			Beginning = beginning;
			Name = name;

			NameBindable.BindValueChanged( v => {
				if ( string.IsNullOrWhiteSpace( v.NewValue ) ) {
					Name = ( ID + 1 ).ToSpreadsheetNotation();
				}
			} );
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
		public string Name {
			get => NameBindable.Value;
			set => NameBindable.Value = value;
		}

		public readonly Bindable<string> NameBindable = new();
	}
}
