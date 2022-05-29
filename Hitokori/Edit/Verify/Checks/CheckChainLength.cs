using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Hitokori.Beatmaps;

namespace osu.Game.Rulesets.Hitokori.Edit.Verify.Checks {
	public class CheckChainLength : ICheck {
		public CheckChainLength () {
			chainLength = new IssueTemplateChainLength( this );
		}

		public IEnumerable<Issue> Run ( BeatmapVerifierContext context ) {
			if ( context.Beatmap.GetHitokoriBeatmap() is not HitokoriBeatmap beatmap )
				yield break;

			foreach ( var i in beatmap.Chains.Values ) {
				if ( i.Beginning.AllInChain.Count() <= 4 )
					yield return chainLength.Create( i );
			}
		}

		public CheckMetadata Metadata { get; } = new CheckMetadata( CheckCategory.Compose, "Problematic chain length" );

		IssueTemplateChainLength chainLength;
		public IEnumerable<IssueTemplate> PossibleTemplates => new IssueTemplate[] {
			chainLength
		};

		private class IssueTemplateChainLength : IssueTemplate {
			public IssueTemplateChainLength ( ICheck check ) 
				: base( check, IssueType.Warning, "Chain {0} has only {1} {2}, which might be too short to read." ) { }

			public Issue Create ( Chain chain ) => new Issue( this, chain.Name, chain.Beginning.AllInChain.Count(), chain.Beginning.AllInChain.Count() == 1 ? "tile" : "tiles" ) {
				Time = chain.Beginning.StartTime,
				HitObjects = chain.Beginning.AllInChain.ToArray()
			};
		}
	}
}
