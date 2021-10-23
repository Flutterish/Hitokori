using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Mods;
using System;

namespace osu.Game.Rulesets.Hitokori.Mods {
	public class HitokoriModOrbitals : Mod, IApplicableToBeatmapProcessor, IApplicableToBeatmapConverter {
		public override string Name => "Adjust Orbitals";
		public override string SettingDescription => $"{OrbitalCount.Value} Orbitals";
		public override string Acronym => "AO";
		public override string Description => "The more the merrier";
		public override double ScoreMultiplier => 1;
		public override ModType Type => ModType.Conversion;

		public override IconUsage? Icon => new IconUsage( '∴' );
		public override bool HasImplementation => true;
		public override Type[] IncompatibleMods => new Type[] { typeof( HitokoriModTapTapDash ) };

		public override bool RequiresConfiguration => true;

		[SettingSource( "Orbital count" )]
		public Bindable<int> OrbitalCount { get; } = new BindableInt( 3 ) {
			MinValue = 3,
			MaxValue = 6,
			Precision = 1
		};

		public void ApplyToBeatmapProcessor ( IBeatmapProcessor beatmapProcessor ) {
			if ( beatmapProcessor is not HitokoriBeatmapProcessor hp ) return;

			hp.ForcedOrbitalCount = OrbitalCount.Value;
		}

		public void ApplyToBeatmapConverter ( IBeatmapConverter beatmapConverter ) {
			if ( beatmapConverter is not HitokoriBeatmapConverter bc ) return;

			bc.ForcedOrbitalCount = OrbitalCount.Value;
		}
	}
}
