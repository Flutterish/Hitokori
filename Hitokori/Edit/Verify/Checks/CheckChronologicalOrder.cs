using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Objects;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Edit.Verify.Checks {
	public class CheckChronologicalOrder : ICheck {
		public CheckChronologicalOrder () {
			reversedChronology = new IssueTemplateReversedChronology( this );
		}

		public IEnumerable<Issue> Run ( BeatmapVerifierContext context ) {
			foreach ( var tile in context.Beatmap.HitObjects.OfType<TilePoint>() ) {
				if ( tile.NextIs( x => x.StartTime < tile.StartTime ) )
					yield return reversedChronology.Create( tile.ToNext );
			}
		}

		public CheckMetadata Metadata { get; } = new CheckMetadata( CheckCategory.HitObjects, "Badly ordered tiles" );

		IssueTemplateReversedChronology reversedChronology;
		public IEnumerable<IssueTemplate> PossibleTemplates => new IssueTemplate[] {
			reversedChronology
		};

		private class IssueTemplateReversedChronology : IssueTemplate {
			public IssueTemplateReversedChronology ( ICheck check )
				: base( check, IssueType.Problem, "Next tile starts before the previous tile." ) { }

			public Issue Create ( TilePointConnector connector ) => new Issue( this ) {
				HitObjects = new HitObject[] { connector.From, connector.To },
				Time = connector.EndTime
			};
		}
	}
}
