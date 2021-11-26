using osu.Game.Rulesets.Hitokori.Orbitals;

namespace osu.Game.Rulesets.Hitokori.Objects.TilePoints
{
    /// <summary>
    /// A <see cref="TilePoint"/> to which the current orbital attaches, after which the next orbital is activated.
    /// </summary>
    public class SwapTilePoint : TilePoint
    {
        public override OrbitalState ModifyOrbitalState(OrbitalState original)
            => original.PivotNth(FromPrevious!.TargetOrbitalIndex, Position);
    }
}
