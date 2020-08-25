using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public abstract class AutoImplementedMod : Mod, IApplicableToBeatmapConverter {
		public override Type[] IncompatibleMods => ModCompatibility.IncompatibleWith( GetType() );

		public override bool HasImplementation => true;

		public void ApplyToBeatmapConverter ( IBeatmapConverter beatmapConverter ) {
			ModCompatibility.ApplyMod( this, beatmapConverter );
		}
	}
}
