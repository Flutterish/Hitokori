using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Objects;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Edit {
	public class HitokoriHitObjectComposer : HitObjectComposer<HitokoriHitObject> {
		public HitokoriBeatmap Beatmap => (HitokoriBeatmap)EditorBeatmap.PlayableBeatmap;
		new public HitokoriEditorPlayfield Playfield => (HitokoriEditorPlayfield)base.Playfield;

		public HitokoriHitObjectComposer ( Ruleset ruleset ) : base( ruleset ) { }

		protected override void LoadComplete () {
			base.LoadComplete();
			EditorBeatmap.HitObjectRemoved += onHitObjectRemoved;
			EditorBeatmap.HitObjectUpdated += onHitObjectUpdated;
		}

		protected override void Update () {
			base.Update();
		}

		private void onHitObjectUpdated ( HitObject obj ) {
			if ( obj is not TilePoint tp ) return;

			tp.Invalidate();
		}

		private void onHitObjectRemoved ( HitObject obj ) {
			if ( obj is not TilePoint tp ) return;

			if ( tp.Previous is null && tp.Next is null ) {
				Beatmap.Chains.Remove( tp.ChainID );
				Playfield.RemoveChain( tp.ChainID );
			}
			else if ( tp.Previous is null ) {
				if ( Playfield.ChainWithID( tp.ChainID ).CurrentTile == tp ) {
					Playfield.RemoveChain( tp.ChainID );
					Playfield.AddChain( tp.Next! );
				}

				Beatmap.Chains[ tp.ChainID ] = tp.Next!;
				tp.Next!.ConstrainPosition = tp.Next.Position;
				tp.Next.OrbitalState = tp.Next.OrbitalState; // constraining the value
				tp.ToNext = null;
			}
			else if ( tp.Next is null ) {
				if ( Playfield.ChainWithID( tp.ChainID ).CurrentTile == tp ) {
					Playfield.RemoveChain( tp.ChainID );
					Playfield.AddChain( tp.Previous! );
				}

				tp.FromPrevious = null;
			}
			else {
				splitNeighbours( tp );
			}
		}

		private void linkNeighbours ( TilePoint tp ) {
			var prev = tp.Previous!;
			Playfield.RemoveChain( tp.ChainID );
			tp.FromPrevious!.To = tp.Next;
			Playfield.AddChain( prev );
		}

		private void splitNeighbours ( TilePoint tp ) {
			Playfield.RemoveChain( tp.ChainID );

			tp.Next!.ConstrainPosition = tp.Next.Position;
			tp.Next.OrbitalState = tp.Next.OrbitalState; // constraining the value

			tp.Next.ChainID = Beatmap.CreateChain( tp.Next );
			foreach ( var i in tp.Next.AllNext ) {
				i.ChainID = tp.Next.ChainID;
			}

			var next = tp.Next;
			var prev = tp.Previous!;

			tp.ToNext = null;
			tp.FromPrevious = null;

			Playfield.AddChain( next );
			Playfield.AddChain( prev );
		}

		protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => Array.Empty<HitObjectCompositionTool>();
	}
}
