using osu.Framework.Bindables;
using osu.Game.Rulesets.Hitokori.Objects;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class Chain {
		public Chain ( TilePoint beginning, string name ) {
			Beginning = beginning;
			Name = name;
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
