using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Hitokori.Beatmaps;
using osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors;
using osu.Game.Rulesets.Hitokori.Edit.Compose.SelectionOverlays;
using osu.Game.Rulesets.Hitokori.Edit.Setup;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Screens.Edit.Compose.Components;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose {
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
				var blueprint = toNextBlueprint?.Connector == connector ? toNextBlueprint
					: null;

				if ( blueprint?.CreateSettingsSection() is Drawable settings ) {
					Composer!.Sidebar.Show();
					Composer.Sidebar.Clear();
					Composer.Sidebar.Add( settings );
					Composer.Sidebar.Add( new ChainSubsection( SelectedChains.Single() ) );
				}
			}

			modifyToNextConnector = new MenuItem( "Edit next connector", () => {
				if ( selectedTilePoint is null || selectedTilePoint.ToNext is null ) return;

				editConnector( selectedTilePoint.ToNext );
			} );

			resetToNextConnector = new MenuItem( "Reset next connector", () => {
				if ( toNextBlueprint is null ) return;

				toNextBlueprint.ResetConstraints();
			} );
		}

		protected override void LoadComplete () {
			base.LoadComplete();

			AddInternal( connectorBlueprintContainer = new Container<ConnectorBlueprint> {
				RelativeSizeAxes = Axes.Both,
				Depth = 1
			} );
			AddInternal( visualizer = new PathVisualizer { Colour = Color4.Yellow } );
			visualizer.Hide();
		}

		private BindablePool<string> bindableStringPool = new();
		private List<Bindable<string>> boundNames = new();

		ConnectorBlueprint? toNextBlueprint;

		protected override void OnSelectionChanged () {
			base.OnSelectionChanged();

			if ( connectorBlueprintContainer.Children.Any( x => !SelectedTiles.Any( y => y.FromPrevious == x.Connector || y.ToNext == x.Connector ) ) ) {
				Composer.Sidebar.Hide();
				connectorBlueprintContainer.Clear();
			}

			if ( SelectedItems.Count == 1 ) {
				selectedTilePoint = SelectedItems[ 0 ] as TilePoint;
				if ( selectedTilePoint is not null ) {
					connectorBlueprintContainer.Clear();
					var blueprint = toNextBlueprint = selectedTilePoint.ToNext?.CreateEditorBlueprint(); // TODO cache these and remove them when the connector is deleted
					if ( blueprint is not null ) {
						connectorBlueprintContainer.Add( blueprint );
						blueprint.Modified += () => {
							Composer!.UpdateVisuals();
						};
						if ( !string.IsNullOrWhiteSpace( blueprint.Tooltip ) ) {
							Composer.Toast.ShowMessage( blueprint.Tooltip );
						}
					}
				}
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

			connectorBlueprintContainer.Alpha = Composer.CurrentTool is SelectTool ? 1 : 0;
		}

		private MenuItem modifyChain;
		private MenuItem modifyToNextConnector;
		private MenuItem resetToNextConnector;
		protected override IEnumerable<MenuItem> GetContextMenuItemsForSelection ( IEnumerable<SelectionBlueprint<HitObject>> selection ) {
			if ( SelectedChains.Count() == 1 ) {
				yield return modifyChain;
			}

			if ( selectedTilePoint is not null ) {
				if ( toNextBlueprint is not null ) {
					yield return modifyToNextConnector;
					if ( toNextBlueprint.CanResetConstraints )
						yield return resetToNextConnector;
				}
			}
		}
	}
}
