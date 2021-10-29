///
/// This is a woraround of a Frame Buffer sizing issue which allows them to scale without bound.
///


using osu.Framework.Graphics;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Graphics {
	public interface IBufferedDrawableWithSizeConstraints : IBufferedDrawable {
		Vector2 MaximumFrameBufferSize { get; }
		bool AllowFrameBufferResizing { get; }
	}
}
