using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Timing;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Patterns;
using osu.Game.Rulesets.Hitokori.Scoring;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Beatmaps {
	public class HitokoriBeatmapConverter : BeatmapConverter<HitokoriHitObject> {
		#region Mods
		public bool DoubleTrouble;
		public bool NoHolds;
		public bool GenerateSpins;
		public bool NoUnhitable;
		public bool Triplets;
		public double Speed = 1;
		#endregion
		public HitokoriBeatmapConverter ( IBeatmap beatmap, Ruleset ruleset ) : base( beatmap, ruleset ) { } // TODO untangle beatmap

		public override bool CanConvert ()
			=> true; // can always convert because it only uses timing. For some reason some modes still dont convert though ( taiko? )

		protected override IEnumerable<HitokoriHitObject> ConvertHitObject ( HitObject original, IBeatmap beatmap ) {
			switch ( original ) {
				case IHasDuration duration:
					return new HoldTile {
						Samples = new List<IList<Audio.HitSampleInfo>> { original.Samples, original.Samples },
						StartTime = original.StartTime,
						EndTime = duration.EndTime
					}.Yield();

				default:
					return new TapTile {
						Samples = original.Samples.Yield().ToList(),
						PressTime = original.StartTime
					}.Yield();
			}
		}

		protected override Beatmap<HitokoriHitObject> ConvertBeatmap ( IBeatmap original ) {
			var beatmap = CreateBeatmap() as HitokoriBeatmap;

			Windows.SetDifficulty( Beatmap.BeatmapInfo.StarDifficulty );
			AngleWindows.SetDifficulty( Beatmap.BeatmapInfo.StarDifficulty );

			beatmap.BeatmapInfo = original.BeatmapInfo;
			beatmap.ControlPointInfo = original.ControlPointInfo;
			beatmap.HitObjects = ConvertHitObjects( original.HitObjects, original );
			PostProcess( beatmap );
			GenerateBreaks( beatmap );

			return beatmap;
		}

		double BPMS => Beatmap.BeatmapInfo.BPM / 60_000;
		public readonly HitokoriHitWindows Windows = new HitokoriHitWindows();
		public readonly HitokoriAngleHitWindows AngleWindows = new HitokoriAngleHitWindows();

		void ConnectHitObjects ( IEnumerable<HitokoriTileObject> hitObjects ) {
			TilePoint PopulatePoint ( TilePoint point ) {
				point.BPMS = BPMS;
				point.SpeedModifier = Speed;
				point.TimeHitWindows = Windows;
				point.AngleHitWindows = AngleWindows;

				return point;
			}

			HitokoriTileObject previous = hitObjects.First();
			TilePoint lastPoint = previous.LastPoint;

			foreach ( var next in hitObjects.Skip( 1 ) ) {
				next.Previous = previous;
				previous.Next = next;
				switch ( next ) {
					case TapTile tile:
						TilePoint point = lastPoint.Then( PopulatePoint( tile.PressPoint ) );
						point.Parent = lastPoint;

						lastPoint = point;
						break;

					case HoldTile hold:
						TilePoint startPoint = lastPoint.Then( PopulatePoint( hold.StartPoint ) );
						startPoint.Parent = lastPoint;

						TilePoint endPoint = PopulatePoint( hold.EndPoint );
						endPoint.Parent = lastPoint;

						lastPoint = endPoint;
						break;
					case SpinTile spin:
						lastPoint.Then( spin.TilePoints.First() );

						var middle = spin.TilePoints[ spin.TilePoints.Count / 2 ];
						bool clockwise = spin.LastPoint.IsClockwise;

						foreach ( var i in spin.TilePoints ) {
							PopulatePoint( i );
							i.IsClockwise = clockwise;
							i.Parent = lastPoint;

							if ( i == middle ) {
								clockwise = !clockwise;
								lastPoint = middle; // swap orbitals in the middle
							}
						}
						lastPoint = spin.LastPoint;
						break;
				}
				previous = next;
			}
		}

		void RemoveUnhitable ( HitokoriBeatmap Beatmap ) {
			List<HitokoriHitObject> objects = Beatmap.HitObjects;
			List<HitokoriHitObject> valid = new List<HitokoriHitObject>();
			double lastEndTime = -10000;

			foreach ( var i in objects ) {
				if ( i.StartTime < lastEndTime ) {
					continue;
				}

				valid.Add( i );
				lastEndTime = i.GetEndTime();
			}

			Beatmap.HitObjects = valid;
		}

		void PostProcess ( HitokoriBeatmap Beatmap ) { //TODO: NOTE this should be in BeatmapProcessor but I cant figure out how to pass mods to it
			Beatmap.HitObjects = Beatmap.HitObjects.OrderBy( x => x.StartTime ).ToList(); // some objects are not always ordered. idk why

			if ( NoUnhitable ) RemoveUnhitable( Beatmap );

			TilePoint firstPoint = new FirstTilePoint {
				BPMS = BPMS,
				IsClockwise = true,
				HitTime = -1000
			};

			HitokoriTileObject StartTile = new StartTile {
				HoldPoint = firstPoint
			};

			ConnectHitObjects( Beatmap.HitObjects.OfType<HitokoriTileObject>().Prepend( StartTile ) );

			PatternGenerator<HitokoriTileObject> generator = new PatternGenerator<HitokoriTileObject>();
			generator.OnBatch += ( original, processed ) => {
				processed = processed.Prepend( original.First().Previous );
				var last = original.Last();
				if ( last.Next != null ) {
					processed = processed.Append( last.Next );
				}

				ConnectHitObjects( processed );
			};

			if ( NoHolds ) generator.AddPattern( new FuckHoldsPatern() );
			generator.AddPattern( new ReverseHoldPattern() );
			if ( DoubleTrouble ) generator.AddPattern( new DoubleTilePattern() );
			if ( GenerateSpins ) generator.AddPattern( new SpinPattern() );
			generator.AddPattern( new StairsPattern() );

			generator.Process( Beatmap.HitObjects.OfType<HitokoriTileObject>() );

			var tiles = StartTile.GetWholeChain().Skip( 1 );
			Beatmap.HitObjects = tiles.Cast<HitokoriHitObject>().ToList();

			if ( Triplets ) {
				foreach ( var tile in tiles.SelectMany( x => x.AllTiles ) ) {
					tile.useTripletAngles = true;
					tile.Refresh();
				}
			}
		}

		void GenerateBreaks ( HitokoriBeatmap Beatmap ) { // TODO: BUG doubleTile mod breaks break generation? Idk it it works at all tbh
			List<BreakPeriod> breaks = new List<BreakPeriod>();

			foreach ( var i in Beatmap.HitObjects.OfType<HitokoriTileObject>() ) {
				if ( i.LastPoint.Duration >= HitokoriPlayfield.MinimumBreakTime ) {
					breaks.Add( new BreakPeriod( i.LastPoint.HitTime, i.LastPoint.HitTime + i.LastPoint.Duration ) );
				}
			}

			Beatmap.Breaks = breaks;
		}

		private List<HitokoriHitObject> ConvertHitObjects ( IReadOnlyList<HitObject> originalHitObjects, IBeatmap original ) {
			List<HitokoriHitObject> hitObjects = new List<HitokoriHitObject>();

			foreach ( var hitObject in originalHitObjects ) {
				hitObjects.AddRange( ConvertHitObject( hitObject, original ) );
			}

			return hitObjects;
		}

		protected override Beatmap<HitokoriHitObject> CreateBeatmap ()
			=> new HitokoriBeatmap();
	}
}
