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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

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
		Drawable? currentConnectorBlueprintSettings;

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
					currentConnectorBlueprintSettings = settings;
					Composer!.Sidebar.Show();
					Composer.Sidebar.Clear();
					Composer.Sidebar.Add( settings );
					Composer.Sidebar.Add( new ChainSubsection( SelectedChains.Single() ) );
				}
			}

			modifyToNextConnector = new MenuItem( "Modify connector" ) {
				Items = new MenuItem[] {
					inspectToNextConnector = new MenuItem( "Inspect", () => {
						if ( selectedTilePoint is null || selectedTilePoint.ToNext is null ) return;

						editConnector( selectedTilePoint.ToNext );
					} ),
					setToNextConnectorType = new MenuItem( "Change type" )
				}
			};
			modifyToNextConnector.Action.BindTo( inspectToNextConnector.Action );

			typeChangers = getAvailableConnectorTypes( GetType().Assembly ).Select( x => (x, new MenuItem( x.Name.Replace( "TilePoint", "" ).Replace( "Connector", "" ).ToSentenceCase(), () => {
				if ( selectedTilePoint is null || selectedTilePoint.ToNext?.GetType() == x )
					return;

				var connector = (TilePointConnector)Activator.CreateInstance( x )!;

				Composer!.Link( selectedTilePoint, selectedTilePoint.Next!, connector );

				if ( Composer.Sidebar.Children.Any( x => x == currentConnectorBlueprintSettings ) ) {
					Composer.Sidebar.Hide();
				}
				OnSelectionChanged();
			} )) ).OrderByDescending( x => x.Item2.Text.Value.ToString() ).ToArray();

			unlinkToNext = new MenuItem( "Next", () => {
				if ( selectedTilePoint is null || selectedTilePoint.ToNext is null ) return;

				Composer!.SplitNeighbours( selectedTilePoint.ToNext );

				if ( Composer!.Sidebar.Children.Any( x => x == currentConnectorBlueprintSettings ) ) {
					Composer.Sidebar.Hide();
				}

				OnSelectionChanged();
				Composer.UpdateVisuals();
			} );

			unlinkFromPrevious = new MenuItem( "Previous", () => {
				if ( selectedTilePoint is null || selectedTilePoint.FromPrevious is null ) return;

				Composer!.SplitNeighbours( selectedTilePoint.FromPrevious );

				OnSelectionChanged();
				Composer.UpdateVisuals();
			} );

			unlinkBoth = new MenuItem( "Both", () => {
				if ( selectedTilePoint is null ) return;

				if ( selectedTilePoint.ToNext is not null )
					unlinkToNext.Action.Value();

				if ( selectedTilePoint.FromPrevious is not null )
					unlinkFromPrevious.Action.Value();
			} );

			unlink = new MenuItem( "Unlink" );
			unlink.Action.BindTo( unlinkBoth.Action );

			link = new MenuItem( "Link", () => {
				if ( SelectedTiles.Count() == 2 ) {
					var ordered = SelectedTiles.OrderBy( x => x.StartTime );

					var a = ordered.First();
					var b = ordered.Last();

					if ( a.Next is null && b.Previous is null ) {
						Composer!.Link( a, b, new TilePointRotationConnector() );
						OnSelectionChanged();
					}
				}
			} );

			resetToNextConnector = new MenuItem( "Reset connector", () => {
				if ( toNextBlueprint is null ) return;

				toNextBlueprint.ResetConstraints();
			} );
		}

		private Type[] getAvailableConnectorTypes ( Assembly assembly ) {
			var connectorType = typeof( TilePointConnector );
			return assembly.GetTypes().Where( x => 
				x.IsAssignableTo( connectorType ) 
				&& !x.IsAbstract 
				&& x.GetConstructors().Any( x => x.GetParameters().Length == 0 ) 
			).ToArray();
		}

		private (Type connectorType, MenuItem menuItem)[] typeChangers; 

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
		string lastTooltip = string.Empty;

		protected override void OnSelectionChanged () {
			base.OnSelectionChanged();

			if ( connectorBlueprintContainer.Children.Any( x => !SelectedTiles.Any( y => y.FromPrevious == x.Connector || y.ToNext == x.Connector ) ) ) {
				Composer.Sidebar.Hide();
				connectorBlueprintContainer.Clear();
			}

			Composer.Toast.HideMessage( lastTooltip );
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

				if ( toNextBlueprint is not null ) {
					if ( string.IsNullOrWhiteSpace( toNextBlueprint.Tooltip ) ) {
						Composer.Toast.HideMessage( lastTooltip );
					}
					else {
						Composer.Toast.ShowMessage( lastTooltip = toNextBlueprint.Tooltip );
					}
				}
			}

			connectorBlueprintContainer.Alpha = Composer.CurrentTool is SelectTool ? 1 : 0;
		}

		private MenuItem modifyChain;
		private MenuItem modifyToNextConnector;
		private MenuItem inspectToNextConnector;
		private MenuItem resetToNextConnector;
		private MenuItem setToNextConnectorType;
		private MenuItem unlink;
		private MenuItem unlinkToNext;
		private MenuItem unlinkFromPrevious;
		private MenuItem unlinkBoth;
		private MenuItem link;
		protected override IEnumerable<MenuItem> GetContextMenuItemsForSelection ( IEnumerable<SelectionBlueprint<HitObject>> selection ) {
			if ( SelectedChains.Count() == 1 ) {
				yield return modifyChain;
			}

			if ( SelectedTiles.Count() == 2 ) {
				var ordered = SelectedTiles.OrderBy( x => x.StartTime );
				
				var a = ordered.First();
				var b = ordered.Last();

				if ( a.Next is null && b.Previous is null ) {
					yield return link;
				}
			}

			if ( selectedTilePoint is not null ) {
				if ( toNextBlueprint is not null ) {
					yield return modifyToNextConnector;
					if ( toNextBlueprint.CanResetConstraints )
						yield return resetToNextConnector;

					setToNextConnectorType.Items = typeChangers.Where( x => x.connectorType != toNextBlueprint.Connector.GetType() )
						.Select( x => x.menuItem ).ToArray();
				}

				IEnumerable<MenuItem> unlinks = Array.Empty<MenuItem>();
				if ( selectedTilePoint.ToNext is not null )
					unlinks = unlinks.Append( unlinkToNext );
				if ( selectedTilePoint.FromPrevious is not null )
					unlinks = unlinks.Append( unlinkFromPrevious );
				if ( unlinks.Count() == 2 )
					unlinks = unlinks.Append( unlinkBoth );

				unlink.Items = unlinks.ToArray();

				if ( unlinks.Any() )
					yield return unlink;
			}
		}
	}
}
