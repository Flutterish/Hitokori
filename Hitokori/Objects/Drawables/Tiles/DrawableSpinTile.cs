using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot;
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
		public override Vector2 NormalizedTilePosition => Tile.LastPoint.Parent.NormalizedTilePosition;
		List<DrawableTilePoint> Points = new List<DrawableTilePoint>();

		public double EndTime => Tile.EndTime;
		public double Duration { get => Tile.Duration; set => Tile.Duration = value; }

		public DrawableSpinTile () : base( null ) {
			this.Center();
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			LifetimeEnd = Tile.EndTime + 1000;
		}

		protected override void AddNestedHitObject ( DrawableHitObject hitObject ) {
			var tile = hitObject as DrawableTilePoint;

			Points.Add( tile );
			AddInternal( tile );

			tile.Position = tile.TilePoint.TilePosition - TilePosition;
			tile.Marker.ConnectFrom( tile.TilePoint.Previous, around: tile.TilePoint.Parent );

			if ( tile.TilePoint == Tile.LastPoint || tile.TilePoint == Tile.TilePoints.First() ) {
				tile.Marker.MarkImportant();
			}
		}

		protected override void ClearNestedHitObjects () {
			foreach ( var i in Points ) {
				RemoveInternal( i );
			}

			Points.Clear();
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( userTriggered ) {
				var next = Points.First( x => !x.Judged );
				Hitokori.OnPress();
				next.TryToHit();
			}
		}

		public bool OnPressed ( KeyBindingPressEvent<HitokoriAction> action ) {
			var next = Points.FirstOrDefault( X => !X.Judged );
			if ( next is null ) return false;
			Playfield.Click( AutoClickType.Press );
			UpdateResult( true );
			return true;
		}
		public void OnReleased ( KeyBindingReleaseEvent<HitokoriAction> action ) { }
	}
}
