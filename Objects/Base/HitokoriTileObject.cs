using osu.Game.Audio;
using osu.Game.Rulesets.Hitokori.Scoring;
using osu.Game.Rulesets.Judgements;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {
	public abstract class HitokoriTileObject : HitokoriHitObject {
		public double SpeedModifier = 1;
		public HitokoriTileObject Next;
		public HitokoriTileObject Previous;

		public abstract IEnumerable<TilePoint> AllTiles { get; }
		public TilePoint FirstPoint => AllTiles.First();
		public TilePoint LastPoint => AllTiles.Last();
		new public List<IList<HitSampleInfo>> Samples = new List<IList<HitSampleInfo>>();

		protected override void CreateNestedHitObjects () {
			int index = 0;
			foreach ( var i in AllTiles ) {
				i.Samples = Samples[ index++ ];
				AddNested( i );
			}
		}

		/// <summary>
		/// Currently no tile other than <see cref="StartTile"/> can be first, therefore <see cref="Previous"/> is never null otherwise
		/// </summary>
		public bool IsFirst => Previous is null;
		public bool IsLast => Next is null;

		public override Judgement CreateJudgement ()
			=> new HitokoriIgnoreJudgement();

		public IEnumerable<HitokoriTileObject> GetWholeChain () {
			List<HitokoriTileObject> chain = new List<HitokoriTileObject>();
			var el = this;

			while ( el.Previous != null ) el = el.Previous;
			chain.Add( el );
			while ( el.Next != null ) {
				el = el.Next;
				chain.Add( el );
			}

			return chain;
		}
	}
}
