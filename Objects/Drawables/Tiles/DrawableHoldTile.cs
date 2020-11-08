using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles {
	public class DrawableHoldTile : HitokoriTile, IHasDuration, IKeyBindingHandler<HitokoriAction> {
		new public readonly HoldTile Tile;

		DrawableTilePoint StartPoint;
		DrawableTilePoint EndPoint;

		ArchedTileConnector Curve;

		public DrawableHoldTile ( HitokoriHitObject hitObject ) : base( hitObject ) {
			Tile = hitObject as HoldTile;
			this.Center();

			AddInternal(
				Curve = new ArchedTileConnector( Tile.StartPoint, Tile.EndPoint, Tile.EndPoint.Parent, Tile.StartPoint.AngleOffset, 1 ) {
					Colour = Tile.StartPoint.Color,
					Position = Tile.StartPoint.Previous.TilePosition - Tile.EndPoint.TilePosition,
					Depth = 1
				}
			);

			NormalizedTilePosition = Tile.EndPoint.NormalizedTilePosition;
		}

		protected override void UpdateInitialTransforms () {
			Curve.Appear( Tile.StartPoint.Duration * 0.75 );

			using ( BeginDelayedSequence( InitialLifetimeOffset, true ) ) {
				Curve.Disappear( Tile.StartPoint.Duration );
			}
		}

		protected override void UpdateHitStateTransforms ( ArmedState state ) {
			LifetimeEnd = Tile.EndTime + 1000;
		}

		protected override void CheckForResult ( bool userTriggered, double timeOffset ) {
			double time = Tile.EndTime + timeOffset;

			if ( Tile.EndPoint.IsNext && ( ReleaseMissed && timeOffset >= 0 ) ) {
				TryToSetResult( EndPoint, HitResult.Miss );
			}
		}

		public double EndTime => ( (IHasDuration)Tile ).EndTime;
		public double Duration { get => ( (IHasDuration)Tile ).Duration; set => ( (IHasDuration)Tile ).Duration = value; }

		HitokoriAction? HoldButton;
		public bool OnPressed ( HitokoriAction action ) { // BUG beatmaps that have a hold tile last end prematurely
			if ( Tile.StartPoint.IsNext ) {
				BeginHold( action );
				return true;
			}

			return false;
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

			if ( Tile.EndPoint.IsNext ) {
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

				StartPoint.OnNewResult += ( a, b ) => { // BUG when rewound the first tile gets missed when the previous tap tile is hit, in general
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
				EndPoint.OnNewResult += ( a, b ) => {
					Hitokori.OnRelease();
					SendClickEvent( AutoClickType.Up );
				};
			}
		}

		protected override void ClearNestedHitObjects () {
			// TODO unify releasing nested hit objects?
			StartPoint.Dispose();
			EndPoint.Dispose();

			StartPoint = null;
			EndPoint = null;
		}
	}
}
