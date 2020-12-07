using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableSpinTile : HitokoriTile, IHasDuration, IKeyBindingHandler<HitokoriAction> { // TODO join paths
		new public readonly SpinTile Tile; // TODO default parameters should be zero
		List<DrawableTilePoint> Points = new List<DrawableTilePoint>(); // TODO rework drawable tile logic

		public double EndTime => ( (IHasDuration)Tile ).EndTime;
		public double Duration { get => ( (IHasDuration)Tile ).Duration; set => ( (IHasDuration)Tile ).Duration = value; }

		public DrawableSpinTile ( HitokoriTileObject tile ) : base( tile ) {
			Tile = tile as SpinTile;
			this.Center();

			NormalizedTilePosition = Tile.LastPoint.Parent.NormalizedTilePosition;
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			LifetimeEnd = Tile.EndTime + 1000;
		}

		protected override void AddNestedHitObject ( DrawableHitObject hitObject ) {
			var tile = hitObject as DrawableTilePoint;

			Points.Add( tile );
			AddInternal( tile );

			tile.Position = tile.TilePoint.TilePosition - TilePosition;
			tile.Marker.ConnectFrom( tile.TilePoint.Previous, tile.TilePoint.Parent );

			if ( tile.TilePoint == Tile.LastPoint || tile.TilePoint == Tile.TilePoints.First() ) {
				tile.Marker.MarkImportant();
			}

			tile.OnNewResult += ( a, b ) => SendClickEvent();
		}

		protected override void ClearNestedHitObjects () {
			foreach ( var i in Points ) {
				i.Dispose();
			}

			Points.Clear();
		}

		public bool OnPressed ( HitokoriAction action ) {
			var next = Points.FirstOrDefault( X => !X.Judged );
			if ( next is null ) return false;
			Hitokori.OnPress();
			next.TryToHit();
			return true;
		}
		public void OnReleased ( HitokoriAction action ) { }
	}
}
