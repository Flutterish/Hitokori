using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Camera;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Hitokori.Edit {
	public class HitokoriEditorPlayfield : HitokoriPlayfield {
		public HitokoriEditorPlayfield ( HitokoriBeatmap beatmap, CameraPath? path = null ) : base( beatmap, path ) {
		
		}

		public override bool TryGetResultFor ( HitObject hitObject, out TileJudgement result ) {
			result = new TileJudgement {
				TimeAbsolute = hitObject.StartTime
			};
			return true;
		}

		protected override HitokoriLifetimeEntry CreateLifetimeEntry ( HitObject hitObject )
			=> new( hitObject, 4000 );
	}
}
