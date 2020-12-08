using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
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

		DrawableTilePoint StartPoint;
		DrawableTilePoint EndPoint;

		CircularTileConnector Curve;

		public DrawableHoldTile ( HitokoriHitObject hitObject ) : base( hitObject ) {
			this.Center();

			AddInternal( Curve = new CircularTileConnector() { Depth = 1 } );
		}
		public DrawableHoldTile () : this( null ) { }

		protected override void OnApply () {
			base.OnApply();
			NormalizedTilePosition = Tile.EndPoint.NormalizedTilePosition;

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
			StartPoint.Marker.Appear();
			EndPoint.Marker.Appear();
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			if ( ( StartPoint.Judged && !EndPoint.Judged ) && ( ReleaseMissed && timeOffset >= 0 ) ) {
				TryToSetResult( EndPoint, HitResult.Miss );
			}
		}

		public double EndTime => ( (IHasDuration)Tile ).EndTime;
		public double Duration { get => ( (IHasDuration)Tile ).Duration; set => ( (IHasDuration)Tile ).Duration = value; }

		HitokoriAction? HoldButton;
		public bool OnPressed ( HitokoriAction action ) { // BUG beatmaps that have a hold tile last end prematurely?
			if ( Clock.ElapsedFrameTime < 0 ) return true;
			if ( StartPoint.Judged ) return false;
			BeginHold( action );
			return true;
		}

		public void OnReleased ( HitokoriAction action ) {
			Release( action );
			HoldButton = null;
		}

		void BeginHold ( HitokoriAction action ) {
			HoldButton = action;

			Hitokori.OnHold();
			StartPoint.TryToHit();
		}

		bool ReleaseMissed;
		void Release ( HitokoriAction action ) {
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
					SendClickEvent( AutoClickType.Down );

					ReleaseMissed = b.Type == HitResult.Miss;
				};
				StartPoint.OnRevertResult += ( a, b ) => {
					HoldButton = null;
					ReleaseMissed = false;
				};
			}
			else if ( tile.TilePoint == Tile.EndPoint ) {
				AddInternal( EndPoint = tile );
				EndPoint.Position = Vector2.Zero;
				EndPoint.OnNewResult += ( a, b ) => {
					Hitokori.OnRelease();
					SendClickEvent( AutoClickType.Up );
				};
			}
		}

		protected override void ClearNestedHitObjects () {
			// TODO unify releasing nested hit objects?
			RemoveInternal( StartPoint );
			RemoveInternal( EndPoint );

			StartPoint = null;
			EndPoint = null;
		}
	}
}
