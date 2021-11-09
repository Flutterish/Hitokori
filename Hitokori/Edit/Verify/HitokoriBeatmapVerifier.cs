using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Hitokori.Edit.Verify.Checks;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Edit.Verify {
	public class HitokoriBeatmapVerifier : IBeatmapVerifier {
        private readonly List<ICheck> checks = new List<ICheck> {
            new CheckChronologicalOrder(),
            new CheckVelocityChanges(),
            new CheckChainLength()
        };

        public IEnumerable<Issue> Run ( BeatmapVerifierContext context ) {
            return checks.SelectMany( check => check.Run( context ) );
        }
	}
}
