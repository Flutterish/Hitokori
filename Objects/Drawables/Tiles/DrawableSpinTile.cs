using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableSpinTile : HitokoriTile, IHasDuration, IKeyBindingHandler<HitokoriAction> {
		new public readonly SpinTile Tile;
		List<DrawableTilePoint> Points = new List<DrawableTilePoint>();

		public double EndTime => ( (IHasDuration)Tile ).EndTime;
		public double Duration { get => ( (IHasDuration)Tile ).Duration; set => ( (IHasDuration)Tile ).Duration = value; }

		public DrawableSpinTile ( HitokoriTileObject tile ) : base( tile ) {
			Tile = tile as SpinTile;
			this.Center();

			NormalizedTilePosition = Tile.LastPoint.Parent.NormalizedTilePosition;
		}

		protected override void UpdateStateTransforms ( ArmedState state ) {
			switch ( state ) {
				case ArmedState.Idle:
					break;

				case ArmedState.Miss:
				case ArmedState.Hit:
					LifetimeEnd = Tile.EndTime + 1000;
					break;
			}
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {

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

			tile.OnNewResult += ( a, b ) => SendAutoClickEvent();
		}

		protected override void ClearNestedHitObjects () {
			foreach ( var i in Points ) {
				i.Dispose();
			}
			
			Points.Clear();
		}

		public bool OnPressed ( HitokoriAction action ) {
			var next = Points.FirstOrDefault( x => x.TilePoint.IsNext );

			if ( next != null ) {
				next.TryToHit();
				return true;
			}
			return false;
		}

		public void OnReleased ( HitokoriAction action ) {
			
		}
	}
}
