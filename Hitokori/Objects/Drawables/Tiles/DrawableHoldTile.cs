using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableHoldTile : HitokoriTile, IHasDuration, IKeyBindingHandler<HitokoriAction> { // TODO hold tiles should reverse at the end, not start. it will make them more readable
		new public HoldTile Tile => HitObject as HoldTile;
		public override Vector2 NormalizedTilePosition => Tile.EndPoint.NormalizedTilePosition;

		public DrawableTilePoint StartPoint { get; private set; }
		public DrawableTilePoint EndPoint { get; private set; }

		CircularTileConnector Curve;

		public DrawableHoldTile () : base( null ) {
			this.Center();

			AddInternal( Curve = new CircularTileConnector() { Depth = 1 } );
		}

		public override void ChildTargeted ( DrawableTilePoint child ) {
			if ( child != StartPoint ) return;
			Curve.FadeColour( AccentColour.Value, 200 );
		}

		protected override void OnApply () {
			base.OnApply();
			Curve.Position = Tile.StartPoint.TilePosition - Tile.EndPoint.TilePosition;
			Curve.From = Tile.StartPoint;
			Curve.Around = Tile.EndPoint.Parent;
			Curve.Angle = Tile.StartPoint.AngleOffset;
			Curve.Colour = Tile.StartPoint.Color;
		}
		protected override void OnFree () {
			base.OnFree();
			Curve.From = null;
			Curve.Around = null;
		}

		protected override void UpdateInitialTransforms () {
			Curve.Appear( Tile.StartPoint.Duration * 0.75 );

			using ( BeginDelayedSequence( InitialLifetimeOffset, true ) ) {
				Curve.Disappear( Tile.StartPoint.Duration, Easing.None );
			}
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			LifetimeEnd = Tile.EndTime + 1000;
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( ( StartPoint.Judged && !EndPoint.Judged ) && ( ReleaseMissed && timeOffset >= 0 ) ) {
				EndPoint.SetResult( HitResult.Miss );
			}
		}

		public double EndTime => Tile.EndTime;
		public double Duration { get => Tile.Duration; set => Tile.Duration = value; }

		HitokoriAction? HoldButton;
		public bool OnPressed ( KeyBindingPressEvent<HitokoriAction> action ) { // BUG beatmaps that have a hold tile last end prematurely?
			if ( Clock.ElapsedFrameTime < 0 ) return true;
			if ( StartPoint.Judged ) return false;
			BeginHold( action.Action );
			return true;
		}

		public void OnReleased ( KeyBindingReleaseEvent<HitokoriAction> action ) {
			Release( action.Action );
			HoldButton = null;
		}

		void BeginHold ( HitokoriAction action ) {
			Playfield.Click( AutoClickType.Down );
			HoldButton = action;

			Hitokori.OnHold();
			StartPoint.TryToHit();
		}

		bool ReleaseMissed;
		void Release ( HitokoriAction action ) {
			if ( action == HoldButton )
				Playfield.Click( AutoClickType.Up );

			if ( action != HoldButton || ReleaseMissed ) {
				return;
			}

			if ( StartPoint.Judged && !EndPoint.Judged ) {
				if ( !EndPoint.TryToHit() ) {
					ReleaseMissed = true;
				}
			}
		}

		protected override void AddNestedHitObject ( DrawableHitObject hitObject ) {
			var tile = hitObject as DrawableTilePoint;

			if ( tile.TilePoint == Tile.StartPoint ) {
				AddInternal( StartPoint = tile );
				StartPoint.Position = Tile.StartPoint.TilePosition - Tile.EndPoint.TilePosition;
				StartPoint.Marker.ConnectFrom( Tile.StartPoint.Previous );

				StartPoint.OnNewResult += ( a, b ) => {
					ReleaseMissed = b.Type == HitResult.Miss;
				};
				StartPoint.OnRevertResult += ( a, b ) => {
					HoldButton = null;
					ReleaseMissed = false;
				};
			}
			else if ( tile.TilePoint == Tile.EndPoint ) {
				AddInternal( EndPoint = tile );
				EndPoint.OnNewResult += ( a, b ) => {
					Hitokori.OnRelease();
				};
			}
		}

		protected override void ClearNestedHitObjects () {
			RemoveInternal( StartPoint, false );
			RemoveInternal( EndPoint, false );

			StartPoint = null;
			EndPoint = null;
		}
	}
}
