using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Edit.Compose.Blueprints;
using osu.Game.Rulesets.Hitokori.Edit.Compose.SelectionOverlays;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Drawables;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Components.TernaryButtons;
using osu.Game.Screens.Edit.Compose;
using osu.Game.Screens.Edit.Compose.Components;
using osuTK;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose {
	[Cached]
	public class HitokoriHitObjectComposer : HitObjectComposer<HitokoriHitObject> {
		public HitokoriBeatmap Beatmap => (HitokoriBeatmap)EditorBeatmap.PlayableBeatmap;
		new public EditorBeatmap EditorBeatmap => base.EditorBeatmap;
		new public HitokoriEditorPlayfield Playfield => (HitokoriEditorPlayfield)base.Playfield;
		public HitObjectCompositionTool CurrentTool => BlueprintContainer.CurrentTool;

		public readonly Container LayerAbovePlayfield;
		public readonly CameraController CameraController;
		public readonly MultiSelectionContainer ProxiedSelectionContainer;
		public readonly EditorSidebar Sidebar;
		public readonly EditorToast Toast;

		[NotNull, MaybeNull]
		private DependencyContainer dependencyContainer;
		protected override IReadOnlyDependencyContainer CreateChildDependencies ( IReadOnlyDependencyContainer parent ) {
			return dependencyContainer = new DependencyContainer( base.CreateChildDependencies( parent ) );
		}

		public HitokoriHitObjectComposer ( Ruleset ruleset ) : base( ruleset ) {
			LayerAbovePlayfield = new Container {
				Name = "Overlays",
				RelativeSizeAxes = Axes.Both,
				Children = new Drawable[] {
					CameraController = new CameraController( this ),
					ProxiedSelectionContainer = new MultiSelectionContainer {
						Alpha = 0.4f
					},
					Sidebar = new EditorSidebar(),
					Toast = new EditorToast( ShowTooltipsToggle )
				}
			};

			ProxiedSelectionContainer.OnUpdate += d => {
				var pos = ComposeScreen.ToSpaceOfOtherDrawable( Vector2.Zero, d.Parent );
				if ( d.Position != pos )
					d.Position = ComposeScreen.ToSpaceOfOtherDrawable( Vector2.Zero, d.Parent );

				if ( d.Size != ComposeScreen.DrawSize )
					d.Size = ComposeScreen.DrawSize;
			};
		}

		private ComposeScreen? composeScreen;
		private ComposeScreen ComposeScreen => composeScreen ??= getContainingComposeScreen();

		private ComposeScreen getContainingComposeScreen () {
			Drawable drawable = this;
			while ( !( drawable is ComposeScreen ) )
				drawable = drawable.Parent;

			return (ComposeScreen)drawable;
		}

		protected override DrawableRuleset<HitokoriHitObject> CreateDrawableRuleset ( Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null )
			=> new DrawableHitokoriRuleset( ruleset, beatmap, mods ) { IsEditor = true };

		public readonly Bindable<TernaryState> ManualCameraToggle = new Bindable<TernaryState>( TernaryState.False );
		public readonly Bindable<TernaryState> PathVisualizerToggle = new Bindable<TernaryState>( TernaryState.True );
		public readonly Bindable<TernaryState> ShowTooltipsToggle = new Bindable<TernaryState>( TernaryState.True );
		protected override IEnumerable<TernaryButton> CreateTernaryButtons () {
			yield return new TernaryButton( ManualCameraToggle, "Manual Camera", () => new SpriteIcon { Icon = FontAwesome.Solid.Video } );
			yield return new TernaryButton( PathVisualizerToggle, "Path Visualizer", () => new SpriteIcon { Icon = FontAwesome.Solid.WaveSquare } );
			yield return new TernaryButton( ShowTooltipsToggle, "Show Tooltips", () => new SpriteIcon { Icon = FontAwesome.Solid.InfoCircle } );
		}

		protected override void LoadComplete () {
			base.LoadComplete();

			var children = new Drawable[ InternalChildren.Count + 1 ];
			var i = 0;
			foreach ( var c in InternalChildren ) {
				if ( c.Name == "Sidebar" ) { // NOTE this is terrible.
					children[ i++ ] = LayerAbovePlayfield;
				}
				children[ i++ ] = c;
			}
			ClearInternal( disposeChildren: false );
			InternalChildren = children;

			EditorBeatmap.HitObjectRemoved += onHitObjectRemoved;
			EditorBeatmap.HitObjectUpdated += onHitObjectUpdated;

			dependencyContainer.CacheAs<HitokoriPlayfield>( Playfield );
			dependencyContainer.CacheAs( Beatmap );

			ManualCameraToggle.BindValueChanged( v => {
				if ( v.NewValue == TernaryState.True ) {
					Playfield.ShouldUpdateCamera = false;
					Toast.ShowMessage( "MMB to pan, scroll to zoom", 4000 );
				}
				else if ( v.NewValue == TernaryState.False ) {
					Playfield.ShouldUpdateCamera = true;
				}
			}, true );
		}

		protected override void Update () {
			base.Update();

			if ( ManualCameraToggle.Value != TernaryState.True )
				Playfield.UpdateCameraViewport( Time.Elapsed );
		}

		private void onHitObjectUpdated ( HitObject obj ) {
			if ( obj is not TilePoint tp ) return;

			ensureValidStartTimes( EditorBeatmap.SelectedHitObjects.OfType<TilePoint>() );

			if ( tp.ToNext is not null )
				tp.ToNext.BPM = Beatmap.ControlPointInfo.TimingPointAt( tp.ToNext.StartTime ).BPM;
			if ( tp.FromPrevious is not null )
				tp.FromPrevious.BPM = Beatmap.ControlPointInfo.TimingPointAt( tp.FromPrevious.StartTime ).BPM;

			tp.Previous?.Invalidate();
			tp.Invalidate();

			Playfield.RemoveChain( tp.ChainID ); // this is done because chains have visual events that need to be reset when timing changes
			Playfield.AddChain( tp );

			UpdateVisuals();
		}

		public void UpdateVisuals () {
			foreach ( DrawableHitokoriHitObject i in Playfield.HitObjectContainer.AliveObjects ) {
				i.UpdateInitialVisuals();
			}
		}

		/// <summary>
		/// Makes sure no <see cref="TilePoint"/> starts before the previous one or ends after the next one.
		/// </summary>
		private void ensureValidStartTimes ( IEnumerable<TilePoint> tilePoints ) {
			double undershootDelta = 0;
			double overshootDelta = 0;

			foreach ( var i in tilePoints ) {
				if ( i.NextIs( x => x.StartTime < i.StartTime ) )
					overshootDelta = Math.Max( overshootDelta, i.StartTime - i.Next.StartTime );

				if ( i.PreviousIs( x => x.StartTime > i.StartTime ) )
					undershootDelta = Math.Max( undershootDelta, i.Previous.StartTime - i.StartTime );
			}

			if ( undershootDelta != 0 && overshootDelta != 0 ) {
				// TODO handle this if needed. it shouldnt be possible though, so whatever
			}
			else if ( undershootDelta != 0 ) {
				foreach ( var i in tilePoints )
					i.StartTime += undershootDelta;
			}
			else if ( overshootDelta != 0 ) {
				foreach ( var i in tilePoints )
					i.StartTime -= overshootDelta;
			}
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

				Beatmap.Chains[ tp.ChainID ].Beginning = tp.Next!;
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
				SplitNeighbours( tp );
			}
		}

		public void LinkNeighbours ( TilePoint tp ) {
			var prev = tp.Previous!;
			Playfield.RemoveChain( tp.ChainID );
			tp.FromPrevious!.To = tp.Next;
			Playfield.AddChain( prev );
		}

		public void SplitNeighbours ( TilePoint tp ) {
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

		public void SplitNeighbours ( TilePointConnector c ) {
			Playfield.RemoveChain( c.From.ChainID );

			c.To.ConstrainPosition = c.To.Position;
			c.To.OrbitalState = c.To.OrbitalState; // constraining the value

			c.To.ChainID = Beatmap.CreateChain( c.To );
			foreach ( var i in c.To.AllNext ) {
				i.ChainID = c.To.ChainID;
			}

			var next = c.To;
			var prev = c.From;

			c.To = null;
			c.From = null;

			Playfield.AddChain( next );
			Playfield.AddChain( prev );
		}

		protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => Array.Empty<HitObjectCompositionTool>();

		protected override ComposeBlueprintContainer CreateBlueprintContainer ()
			=> new HitokoriComposeBlueprintContainer( this );
	}
}
