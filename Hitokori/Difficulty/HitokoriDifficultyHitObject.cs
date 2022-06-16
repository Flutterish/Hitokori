using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Objects;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Difficulty {
	public class HitokoriDifficultyHitObject : DifficultyHitObject {
		public bool ChangedDirection { get; private set; }

		/// <summary>
		/// The map's BPM at the moment of this object
		/// </summary>
		public double BPM { get; private set; }

		/// <summary>
		/// The angle traveled to hit this object
		/// </summary>
		public double HitAngle { get; private set; }

		/// <summary>
		/// The angle that this object is held, if applicable
		/// </summary>
		public double? HoldAngle { get; private set; }

		public HitokoriDifficultyHitObject ( HitObject hitObject, HitObject lastObject, double bpm, double clockRate, 
			List<DifficultyHitObject> objects, int index ) : base( hitObject, lastObject, clockRate, objects, index ) {

			HitokoriTileObject hitokoriObject = (HitokoriTileObject)hitObject;
			HitokoriTileObject lastHitokoriObject = (HitokoriTileObject)lastObject;

			BPM = bpm;
			ChangedDirection = hitokoriObject.FirstPoint.ChangedDirection;
			HitAngle = lastHitokoriObject.LastPoint.AngleOffset;
			if ( hitokoriObject is HoldTile ) {
				HoldAngle = ( (HoldTile)hitokoriObject ).FirstPoint.AngleOffset;
			}
		}
	}
}
