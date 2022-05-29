using osu.Framework.Localisation;
using osu.Game.Rulesets.Hitokori.Beatmaps;

namespace osu.Game.Rulesets.Hitokori.Edit.Setup {
	public class ChainsSubsection : SetupSubsection {
		HitokoriBeatmap beatmap;
		BindableDictionary<int, Chain> Chains = new();
		public ChainsSubsection ( HitokoriBeatmap beatmap ) {
			this.beatmap = beatmap;
		}

		public override LocalisableString Title => "Chains";
		Dictionary<Chain, ChainSubsection> subsections = new();

		protected override void LoadComplete () {
			base.LoadComplete();

			Chains.BindTo( beatmap.Chains );
			Chains.BindCollectionChanged( ( _, v ) => {
				if ( v.OldItems is not null ) {
					foreach ( var i in v.OldItems ) {
						Remove( subsections[ i.Value ] );
						subsections.Remove( i.Value );
					}
				}

				if ( v.NewItems is not null ) {
					foreach ( var i in v.NewItems ) {
						var subsection = new ChainSubsection( i.Value );
						subsections.Add( i.Value, subsection );
						Add( subsection );
					}
				}
			}, true );
		}
	}
}
