using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.UI {
	[Cached]
	public class HitokoriPlayfield : Playfield {
		[Cached]
		public readonly SparklePool SparklePool = new SparklePool();
		[Cached]
		public readonly PathPool PathPool = new PathPool();

		/// <summary>
		/// Camera position. Used because offsetting containers clips children.
		/// </summary>
		AnimatedVector CameraPosition;
		BindableDouble CameraSpeed = new( 1 );

		public readonly Container Everything;
		public readonly Container SFX;

		JudgementContainer<DrawableHitokoriJudgement> Judgements;
		HitObjectContainer Tiles;
		[Cached]
		public readonly DrawableHitokori Hitokori;

		private bool reverseSpin;

		Bot AutoBot;
		Beatmap<HitokoriHitObject> beatmap;
		public HitokoriPlayfield ( bool auto, bool triplets, bool reverseSpin, Beatmap<HitokoriHitObject> beatmap ) {
			this.beatmap = beatmap;
			this.reverseSpin = reverseSpin;
			CameraPosition = new AnimatedVector( parent: this );

			InternalChildren = new Drawable[] {
				Everything = new Container().Center(),
				SFX = new Container().Center()
			};

			Everything.AddRange( new Drawable[] {
				Hitokori = new DrawableHitokori { Depth = -1 }.Center(),
				Tiles = HitObjectContainer.Center(),
				Judgements = new JudgementContainer<DrawableHitokoriJudgement>().Center()
			} );

			if ( Auto = auto ) {
				AddInternal( AutoBot = new Bot {
					Position = new osuTK.Vector2( 100, -100 ),
					Anchor = Anchor.BottomLeft,
					Origin = Anchor.Centre
				} );
			}

			if ( triplets ) {
				Hitokori.AddTriplet();
			}

			RegisterPool<TapTile, DrawableTapTile>( 20 );
			RegisterPool<HoldTile, DrawableHoldTile>( 10 );
			RegisterPool<SpinTile, DrawableSpinTile>( 3 );
		}
		protected override HitObjectLifetimeEntry CreateLifetimeEntry ( HitObject hitObject ) => new HitokoriHitObjectLifetimeEntry( hitObject );

		public bool Auto = false;
		protected override void LoadComplete () {
			base.LoadComplete();
			ScheduleAfterChildren( () => {
				var head = beatmap.HitObjects.First();
				if ( head is HitokoriTileObject Head ) {
					using ( BeginAbsoluteSequence( Head.Previous.LastPoint.HitTime, true ) ) {
						Hitokori.Swap( Head.Previous.LastPoint );
					}
				}
				else throw new InvalidOperationException( "What the fuck" );
			} );
		}
		protected override void UpdateAfterChildren () {
			if ( reverseSpin ) {
				Everything.Rotation = -Hitokori.StableAngle.ToDegreesF() * 0.7f;
			}
			else {
				Everything.Rotation = 0;
			}

			UpdateOffsets();
		}

		private void UpdateOffsets () {
			// TODO ISSUE #5 when rewinding time, camera is often far from the gameplay
			var followTiles = Tiles.AliveObjects.OfType<HitokoriTile>()
				.Where( x => x.LifetimeStart <= Clock.CurrentTime && x.LifetimeEnd >= Clock.CurrentTime );

			var averagePosition = ( followTiles.AverageOr( x => x.TilePosition, Hitokori.TilePosition ) + Hitokori.TilePosition ) / 2;
			CameraPosition.AnimateTo( averagePosition, 300 / CameraSpeed.Value );

			foreach ( var tile in Tiles.AliveObjects.OfType<IHasTilePosition>().Concat( Judgements ).Append( Hitokori ) ) {
				if ( tile is Drawable drawable ) drawable.Position = tile.TilePosition - CameraPosition;
			}
		}

		protected override void OnNewDrawableHitObject ( DrawableHitObject drawableHitObject ) {
			base.OnNewDrawableHitObject( drawableHitObject );

			if ( drawableHitObject is not HitokoriTile tile ) return;
			tile.OnNewResult += OnTileResult;
		}

		public void Click ( AutoClickType type ) {
			if ( type == AutoClickType.Down )
				AutoBot?.Hold();
			else if ( type == AutoClickType.Up )
				AutoBot?.Release();
			else if ( type == AutoClickType.Press )
				AutoBot?.Press();
		}

		private void OnTileResult ( DrawableHitObject obj, JudgementResult result ) {
			if ( obj is HitokoriTile tile ) {
				if ( tile.Tile.LastPoint.Duration > MinimumBreakTime || tile.Tile.IsLast ) {
					Hitokori.Contract();
					AutoBot?.AllowGhosting();

					if ( !tile.Tile.IsLast ) {
						this.Delay( tile.Tile.LastPoint.Duration - 1000 ).Schedule( () => {
							Hitokori.Expand();
							AutoBot?.ForbidGhosting();
						} );
					}
				}
			}
			else if ( obj is DrawableTilePoint point ) {
				Judgements.Add( new DrawableHitokoriJudgement( result, point ) );
			}
		}

		public static readonly double MinimumBreakTime = 1000;

		[BackgroundDependencyLoader( true )]
		private void load ( HitokoriSettingsManager config ) {
			config?.BindWith( HitokoriSetting.CameraSpeed, CameraSpeed );
		}

		protected override void Dispose ( bool isDisposing ) {
			base.Dispose( isDisposing );

			SparklePool?.Dispose();
			PathPool?.Dispose();
		}
	}
}
