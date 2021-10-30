using osu.Framework.Allocation;
using osu.Game.Rulesets.Hitokori.Edit.SelectionOverlays;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Screens.Edit.Compose.Components;
using osuTK;
using osuTK.Graphics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Edit {
	public class HitokoriSelectionHandler : EditorSelectionHandler {
		[Resolved, MaybeNull, NotNull]
		public HitokoriHitObjectComposer Composer { get; private set; }

		[MaybeNull, NotNull]
		PathVisualizer visualizer;
		TilePoint? selectedTilePoint;

		protected override void LoadComplete () {
			base.LoadComplete();

			AddInternal( visualizer = new PathVisualizer { Colour = Color4.Yellow } );
			visualizer.Hide();
		}

		protected override void OnSelectionChanged () {
			base.OnSelectionChanged();

			if ( SelectedItems.Count == 1 ) {
				selectedTilePoint = SelectedItems[ 0 ] as TilePoint;
			}
			else {
				selectedTilePoint = null;
			}

			visualizer.VisualizedConnector.Value = selectedTilePoint?.ToNext;

			var chains = SelectedItems.OfType<TilePoint>().Select( x => x.ChainID ).Distinct()
				.OrderBy( x => Composer.Beatmap.Chains[ x ].Beginning.StartTime ).ThenBy( x => x );

			if ( SelectedItems.Count == 1 ) {
				SelectionBox.Text += $" | Chain {Composer.Beatmap.Chains[ chains.First() ].Name}";
			}
			else if ( chains.Count() > 1 ) {
				SelectionBox.Text += $" | Chains: {string.Join( ", ", chains.Select( x => Composer.Beatmap.Chains[ x ].Name ) )}";
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
	}
}
