using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Edit.Connectors;
using osu.Game.Rulesets.Hitokori.Edit.SelectionOverlays;
using osu.Game.Rulesets.Hitokori.Edit.Setup;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Screens.Edit.Compose.Components;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Edit {
	public class HitokoriSelectionHandler : EditorSelectionHandler {
		[Resolved, MaybeNull, NotNull]
		public HitokoriHitObjectComposer Composer { get; private set; }

		[MaybeNull, NotNull]
		PathVisualizer visualizer;
		TilePoint? selectedTilePoint;
		public IEnumerable<Chain> SelectedChains => SelectedTiles.Select( x => x.ChainID ).Distinct().Select( x => Composer.Beatmap.Chains[ x ] );
		public IEnumerable<TilePoint> SelectedTiles => SelectedItems.OfType<TilePoint>();

		[MaybeNull, NotNull]
		Container<ConnectorBlueprint> connectorBlueprintContainer;

		public HitokoriSelectionHandler () {
			modifyChain = new MenuItem( "Modify chain", () => {
				if ( SelectedChains.Count() != 1 ) return;

				Composer!.Sidebar.Show();
				Composer.Sidebar.Clear();
				Composer.Sidebar.Add( new ChainSubsection( SelectedChains.Single() ) { ShowSide = false } );
			} );

			void editConnector ( TilePointConnector connector ) {
				var blueprint = connector.CreateEditorBlueprint();

				if ( blueprint.CreateSettingsSection() is Drawable settings ) {
					Composer!.Sidebar.Show();
					Composer.Sidebar.Clear();
					Composer.Sidebar.Add( settings );
				}

				connectorBlueprintContainer!.Clear();
				connectorBlueprintContainer.Add( blueprint );
			}

			modifyToNextConnector = new MenuItem( "Edit next connector", () => {
				if ( selectedTilePoint is null || selectedTilePoint.ToNext is null ) return;

				editConnector( selectedTilePoint.ToNext );
			} );
			modifyFromPreviousConnector = new MenuItem( "Edit previous connector", () => {
				if ( selectedTilePoint is null || selectedTilePoint.FromPrevious is null ) return;

				editConnector( selectedTilePoint.FromPrevious );
			} );
		}

		protected override void LoadComplete () {
			base.LoadComplete();

			Composer!.LayerAbovePlayfield.Add( connectorBlueprintContainer = new Container<ConnectorBlueprint> {
				RelativeSizeAxes = Axes.Both
			} );
			AddInternal( visualizer = new PathVisualizer { Colour = Color4.Yellow } );
			visualizer.Hide();
		}

		private BindablePool<string> bindableStringPool = new();
		private List<Bindable<string>> boundNames = new();

		protected override void OnSelectionChanged () {
			base.OnSelectionChanged();

			if ( connectorBlueprintContainer.Children.Any( x => !SelectedTiles.Any( y => y.FromPrevious == x.Connector || y.ToNext == x.Connector ) ) ) {
				Composer.Sidebar.Hide();
				connectorBlueprintContainer.Clear();
			}

			if ( SelectedItems.Count == 1 ) {
				selectedTilePoint = SelectedItems[ 0 ] as TilePoint;
			}
			else {
				selectedTilePoint = null;
			}

			visualizer.VisualizedConnector.Value = selectedTilePoint?.ToNext;

			foreach ( var i in boundNames ) {
				bindableStringPool.Return( i );
			}
			boundNames.Clear();
			foreach ( var i in SelectedChains ) {
				var bindable = bindableStringPool.Rent();
				boundNames.Add( bindable );
				bindable.BindTo( i.NameBindable );
				bindable.ValueChanged += onSelectedChainNameChanged;
			}
			updateSelectionText();
		}

		private void onSelectedChainNameChanged ( ValueChangedEvent<string> obj ) {
			updateSelectionText();
		}

		private void updateSelectionText () {
			SelectionBox.Text = SelectedItems.Count.ToString();

			var chains = SelectedChains.OrderBy( x => x.Beginning.StartTime ).ThenBy( x => x.Beginning.ChainID );

			if ( chains.Count() == 1 ) {
				SelectionBox.Text += $" | Chain {chains.First().Name.Trim()}";
			}
			else if ( chains.Count() > 1 ) {
				SelectionBox.Text += $" | Chains: {string.Join( ", ", chains.Select( x => x.Name.Trim() ) )}";
			}
		}

		protected override void Update () {
			base.Update();

			if ( selectedTilePoint is not null ) {
				visualizer.Scale = new Vector2( (float)Composer.Playfield.CameraScale.Value );
				visualizer.Position = ToLocalSpace( Composer.Playfield.ScreenSpacePositionOf( (Vector2)selectedTilePoint.Position ) );
				visualizer.TilePosition = (Vector2)selectedTilePoint.Position;
			}
		}

		private MenuItem modifyChain;
		private MenuItem modifyToNextConnector;
		private MenuItem modifyFromPreviousConnector;
		protected override IEnumerable<MenuItem> GetContextMenuItemsForSelection ( IEnumerable<SelectionBlueprint<HitObject>> selection ) {
			if ( SelectedChains.Count() == 1 ) {
				yield return modifyChain;
			}

			if ( selectedTilePoint is not null ) {
				if ( selectedTilePoint.ToNext is not null ) {
					yield return modifyToNextConnector;
				}
				if ( selectedTilePoint.FromPrevious is not null ) {
					yield return modifyFromPreviousConnector;
				}
			}
		}
	}
}
