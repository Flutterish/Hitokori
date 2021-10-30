using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Screens.Edit.Setup;

namespace osu.Game.Rulesets.Hitokori.Edit.Setup {
	public class HitokoriSetupSection : RulesetSetupSection {
		public HitokoriSetupSection ( RulesetInfo rulesetInfo ) : base( rulesetInfo ) {
			
		}

		new protected HitokoriBeatmap Beatmap => (HitokoriBeatmap)base.Beatmap.PlayableBeatmap;

		protected override void LoadComplete () {
			base.LoadComplete();

			Add( new ChainsSubsection( Beatmap ) );
		}
	}
}
