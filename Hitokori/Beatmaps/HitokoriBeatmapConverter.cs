using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;
using System;
using System.Collections.Generic;
using System.Threading;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmapConverter : BeatmapConverter<HitokoriHitObject> {
		public HitokoriBeatmapConverter ( IBeatmap beatmap, Ruleset ruleset ) : base( beatmap, ruleset ) {
		}

		public override bool CanConvert () => true;

		protected override IEnumerable<HitokoriHitObject> ConvertHitObject ( HitObject original, IBeatmap beatmap, CancellationToken cancellationToken ) {
			cancellationToken.ThrowIfCancellationRequested();

			if ( original is IHasDuration dur ) {
				if ( dur.Duration < beatmap.BeatLengthAt( original.StartTime ) ) {
					yield return new SwapTilePoint {
						Samples = original.Samples,
						StartTime = original.StartTime
					};
					yield return new SwapTilePoint {
						Samples = original.Samples,
						StartTime = dur.EndTime
					};
				}
				else {
					yield return new PassThroughTilePoint {
						Samples = original.Samples,
						StartTime = original.StartTime
					};
				}
			}
			else {
				yield return new SwapTilePoint {
					Samples = original.Samples,
					StartTime = original.StartTime
				};
			}
		}

		protected override Beatmap<HitokoriHitObject> ConvertBeatmap ( IBeatmap original, CancellationToken cancellationToken ) {
			var beatmap = CreateBeatmap();

			beatmap.BeatmapInfo = original.BeatmapInfo;
			beatmap.ControlPointInfo = original.ControlPointInfo;
			beatmap.Breaks = original.Breaks;

			beatmap.HitObjects.Add( new NoJudgementTilePoint {
				Position = Vector2d.Zero,
				OrbitalState = new Orbitals.OrbitalState( new Vector2d[] {
					(0.0).AngleToVector(0.5),
					(Math.PI).AngleToVector(0.5)
				} ),
				StartTime = 0
			} );

			foreach ( var i in original.HitObjects ) {
				beatmap.HitObjects.AddRange( ConvertHitObject( i, original, cancellationToken ) );
			}

			beatmap.HitObjects.Sort( (a,b) => Math.Sign(a.StartTime - b.StartTime) );

			return beatmap;
		}
	}
}
