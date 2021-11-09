using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Edit.Verify.Checks {
	public class CheckVelocityChanges : ICheck {
		public CheckVelocityChanges () {
			velocityChange = new IssueTemplateVelocityChange( this, IssueType.Warning );
			massiveVelocityChange = new IssueTemplateVelocityChange( this, IssueType.Problem );
		}

		public IEnumerable<Issue> Run ( BeatmapVerifierContext context ) {
			foreach ( var i in context.Beatmap.HitObjects.OfType<TilePoint>() ) {
				if ( i.SpeedDifference is double v ) {
					if ( Math.Abs( v ) > 0.7 )
						yield return massiveVelocityChange.Create( i );
					else if ( Math.Abs( v ) > 0.45 )
						yield return velocityChange.Create( i );
				}
			}
		}

		public CheckMetadata Metadata { get; } = new CheckMetadata( CheckCategory.Compose, "Big velocity change" );

		IssueTemplateVelocityChange velocityChange;
		IssueTemplateVelocityChange massiveVelocityChange;
		public IEnumerable<IssueTemplate> PossibleTemplates => new IssueTemplate[] {
			velocityChange,
			massiveVelocityChange
		};

		private class IssueTemplateVelocityChange : IssueTemplate {
			public IssueTemplateVelocityChange ( ICheck check, IssueType severity )
				: base( check, severity, "Velocity changed by {0:0.00%}. Consider a more gradual change." ) { }

			public Issue Create ( TilePoint tp ) => new Issue( this, tp.SpeedDifference!.Value ) {
				HitObjects = new HitObject[] { tp, tp.Previous!, tp.Next! },
				Time = tp.StartTime
			};
		}
	}
}
