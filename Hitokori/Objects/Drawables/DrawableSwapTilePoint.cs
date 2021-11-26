using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Hitokori.Input;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables
{
    public class DrawableSwapTilePoint : DrawableTilePointWithConnections<SwapTilePoint>, IKeyBindingHandler<HitokoriAction>
    {
        public DrawableSwapTilePoint()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered)
            {
                var result = HitObject.HitWindows.ResultFor(timeOffset);

                if (result is HitResult.None)
                {
                    // TODO some animation to show the "too early"
                }
                else
                {
                    ApplyResult(j => j.Type = result);
                }
            }
            else if (!HitObject.HitWindows.CanBeHit(timeOffset))
            {
                ApplyResult(j => j.Type = HitResult.Miss);
            }
        }

        public bool OnPressed(KeyBindingPressEvent<HitokoriAction> action)
        {
            if (Judged) return false;

            UpdateResult(true);
            return true;
        }

        public void OnReleased(KeyBindingReleaseEvent<HitokoriAction> action) { }
    }
}
