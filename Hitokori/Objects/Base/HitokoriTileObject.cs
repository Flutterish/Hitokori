using osu.Game.Audio;
using osu.Game.Rulesets.Hitokori.Scoring;
using osu.Game.Rulesets.Judgements;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace osu.Game.Rulesets.Hitokori.Objects.Base {
	public abstract class HitokoriTileObject : HitokoriHitObject {
		public HitokoriTileObject Next;
		public HitokoriTileObject Previous;

		public abstract IEnumerable<TilePoint> AllTiles { get; }
		public TilePoint FirstPoint => AllTiles.First();
		public TilePoint LastPoint => AllTiles.Last();
		new public List<IList<HitSampleInfo>> Samples = new List<IList<HitSampleInfo>>(); // TODO idk this seems sketch

		protected override void CreateNestedHitObjects ( CancellationToken cancellationToken ) {
			int index = 0;
			foreach ( var i in AllTiles ) {
				i.Samples = Samples[ index++ ];
				AddNested( i );

				cancellationToken.ThrowIfCancellationRequested();
			}
		}

		public bool IsFirst => Previous is null;
		public bool IsLast => Next is null;

		public override Judgement CreateJudgement ()
			=> new HitokoriIgnoreJudgement();

		public IEnumerable<HitokoriTileObject> GetWholeChain () {
			var el = this;

			while ( el.Previous != null ) el = el.Previous;
			while ( el != null ) {
				yield return el;
				el = el.Next;
			}
		}
	}
}
