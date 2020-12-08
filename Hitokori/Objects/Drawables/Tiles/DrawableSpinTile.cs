using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osuTK;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableSpinTile : HitokoriTile, IHasDuration, IKeyBindingHandler<HitokoriAction> { // TODO join paths
		new public SpinTile Tile => HitObject as SpinTile;
		List<DrawableTilePoint> Points = new List<DrawableTilePoint>();

		public double EndTime => ( (IHasDuration)Tile ).EndTime;
		public double Duration { get => ( (IHasDuration)Tile ).Duration; set => ( (IHasDuration)Tile ).Duration = value; }

		public DrawableSpinTile ( HitokoriTileObject tile ) : base( tile ) {
			this.Center();
		}
		public DrawableSpinTile () : this( null ) { }
		protected override void OnApply () {
			base.OnApply();
		}
		public override Vector2 NormalizedTilePosition {
			get => Tile.LastPoint.Parent.NormalizedTilePosition;
			set => base.NormalizedTilePosition = value;
		}
		protected override void OnFree () {
			base.OnFree();
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			LifetimeEnd = Tile.EndTime + 1000;
			foreach ( var i in Points ) {
				i.Marker.Appear();
			}
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
				RemoveInternal( i );
			}

			Points.Clear();
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( userTriggered ) {
				var next = Points.First( X => !X.Judged );
				Hitokori.OnPress();
				next.TryToHit();
			}
		}

		public bool OnPressed ( HitokoriAction action ) {
			var next = Points.FirstOrDefault( X => !X.Judged );
			if ( next is null ) return false;
			UpdateResult( true );
			return true;
		}
		public void OnReleased ( HitokoriAction action ) { }
	}
}
