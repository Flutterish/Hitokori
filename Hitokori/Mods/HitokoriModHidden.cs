using osu.Framework.Graphics;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModHidden : ModHidden {
		public override double ScoreMultiplier => 1.10;

		public override string Description => "Just like your feelings";

		protected override void ApplyNormalVisibilityState ( DrawableHitObject hitObject, ArmedState state ) {
			if ( hitObject is DrawableTilePoint tile ) {
				using ( tile.BeginAbsoluteSequence( tile.TilePoint.HitTime - 700 ) ) {
					tile.FadeOut( 300 );
				}
			}
			else if ( hitObject is DrawableHoldTile hold ) {
				using ( hold.BeginAbsoluteSequence( hold.Tile.StartPoint.HitTime - 500 ) ) {
					hold.FadeOut( 300 );
				}
			}
		}

		protected override void ApplyIncreasedVisibilityState ( DrawableHitObject hitObject, ArmedState state ) { }
	}
}
