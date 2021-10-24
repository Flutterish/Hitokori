using osu.Game.Rulesets.Hitokori.Collections;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Camera {
	public class CameraPath {
		/// <summary>
		/// Position (centre)
		/// </summary>
		public readonly AnimatedValue<Vector2> Position = new();
		/// <summary>
		/// Bounding box size. A simple scale cannot be used as it depends on playfields size
		/// </summary>
		public readonly AnimatedValue<Vector2> Size = new();
		/// <summary>
		/// Camera rotation. Since it is the camera that is rotated, the playfield is rotated the other direction
		/// </summary>
		public readonly AnimatedValue<float> Rotation = new();
	}
}
