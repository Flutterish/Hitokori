using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Edit.Compose.Blueprints;
using osu.Game.Rulesets.Hitokori.Edit.Compose.SelectionOverlays;
using osu.Game.Rulesets.Hitokori.Edit.Compose.Tools;
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
using System.Diagnostics.CodeAnalysis;

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

		protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => new HitObjectCompositionTool[] {
			new TilePointCompositionTool( "Add Tile" )
		};

		protected override void LoadComplete () {
			base.LoadComplete();
			AddInternal( LayerAbovePlayfield );

			EditorBeatmap.HitObjectAdded += onHitObjectAdded;
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

		new public void Schedule ( Action action )
			=> base.Schedule( action );

		private void onHitObjectAdded ( HitObject obj ) {
			if ( obj is not TilePoint tp ) return;

			if ( tp.Previous is null && tp.Next is null ) {
				tp.ChainID = Beatmap.CreateChain( tp );
				Playfield.AddChain( tp );
			}

			EditorBeatmap.Update( obj );
		}

		private void onHitObjectUpdated ( HitObject obj ) {
			if ( obj is not TilePoint tp ) return;

			ensureValidStartTimes( EditorBeatmap.SelectedHitObjects.OfType<TilePoint>() );

			updateTilePoint( tp );
			UpdateOrbitals( tp ); // this is done because chains have visual events that need to be reset when timing changes
			UpdateVisuals();
		}

		private void onHitObjectRemoved ( HitObject obj ) {
			if ( obj is not TilePoint tp ) return;

			if ( tp.Previous is null && tp.Next is null ) {
				Beatmap.Chains.Remove( tp.ChainID );
				Playfield.RemoveChain( tp.ChainID );
			}
			else if ( tp.Previous is null ) {
				Playfield.RemoveChain( tp.ChainID );
				var attach = tp == obj ? tp.Next! : tp;

				Beatmap.Chains[ tp.ChainID ].Beginning = tp.Next!;
				tp.Next!.ConstrainPosition = tp.Next.Position;
				tp.Next.ConstrainOrbitalState = tp.Next.OrbitalState;
				tp.ToNext = null;

				Playfield.AddChain( attach );
			}
			else if ( tp.Next is null ) {
				Playfield.RemoveChain( tp.ChainID );
				var attach = tp == obj ? tp.Previous! : tp;

				tp.FromPrevious = null;

				Playfield.AddChain( attach );
			}
			else {
				SplitNeighbours( tp );
			}
		}

		private void updateTilePoint ( TilePoint tp ) {
			if ( tp.ToNext is not null )
				tp.ToNext.BPM = Beatmap.ControlPointInfo.TimingPointAt( tp.ToNext.StartTime ).BPM;
			if ( tp.FromPrevious is not null )
				tp.FromPrevious.BPM = Beatmap.ControlPointInfo.TimingPointAt( tp.FromPrevious.StartTime ).BPM;

			tp.Previous?.Invalidate();
			tp.Invalidate();
		}

		public void UpdateVisuals () {
			foreach ( DrawableHitokoriHitObject i in Playfield.HitObjectContainer.AliveObjects ) {
				if ( i.HitObject is not TilePoint tp ) 
					continue;

				updateTilePoint( tp );
				i.UpdateInitialVisuals();
			}
		}

		public void UpdateOrbitals ( int ID ) {
			Playfield.RemoveChain( ID );
			Playfield.AddChain( Beatmap.Chains[ ID ].Beginning );
		}

		public void UpdateOrbitals ( TilePoint tp ) {
			Playfield.RemoveChain( tp.ChainID );
			Playfield.AddChain( tp );
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

		public void LinkNeighbours ( TilePoint tp ) {
			var prev = tp.Previous!;
			Playfield.RemoveChain( tp.ChainID );
			tp.FromPrevious!.To = tp.Next;
			Playfield.AddChain( prev );

			UpdateVisuals();
		}

		public void Link ( TilePoint from, TilePoint to, TilePointConnector connector, bool unconstrain = true ) {
			from.ToNext = null;

			connector.To = to;
			connector.From = from;

			connector.BPM = Beatmap.ControlPointInfo.TimingPointAt( connector.StartTime ).BPM;

			if ( unconstrain ) {
				to.ConstrainableOrbitalState.ReleaseConstraint();
				to.ConstrainablePosition.ReleaseConstraint();
			}

			if ( from.ChainID != to.ChainID ) {
				Playfield.RemoveChain( to.ChainID );
				Beatmap.Chains.Remove( to.ChainID );

				foreach ( var i in to.AllInChain ) {
					i.ChainID = from.ChainID;
				}

				UpdateOrbitals( from );
			}

			connector.Invalidate();
			connector.ApplyDefaults();
			UpdateVisuals();
		}

		public void SplitNeighbours ( TilePoint tp ) {
			Playfield.RemoveChain( tp.ChainID );

			tp.Next!.ConstrainPosition = tp.Next.Position;
			tp.Next.ConstrainOrbitalState = tp.Next.OrbitalState;

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

			UpdateVisuals();
		}

		public void SplitNeighbours ( TilePointConnector c ) {
			Playfield.RemoveChain( c.From.ChainID );

			c.To.ConstrainPosition = c.To.Position;
			c.To.ConstrainOrbitalState = c.To.OrbitalState;

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

			UpdateVisuals();
		}

		protected override ComposeBlueprintContainer CreateBlueprintContainer ()
			=> new HitokoriComposeBlueprintContainer( this );
	}
}
