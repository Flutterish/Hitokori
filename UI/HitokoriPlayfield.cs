using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using SixLabors.ImageSharp.Formats;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.UI {
	public class HitokoriPlayfield : Playfield {
		/// <summary>
		/// Camera position. Used because offsetting containers clips children.
		/// </summary>
		AnimatedVector CameraPosition;
		Bindable<CameraFollowMode> FollowMode = new Bindable<CameraFollowMode>();
		Bindable<double> CameraSpeed = new Bindable<double>( 300 );

		JudgementContainer<DrawableHitokoriJudgement> Judgements;
		HitObjectContainer Tiles;
		public readonly DrawableHitokori Hitokori;

		Bot AutoBot;
		public HitokoriPlayfield ( bool auto ) {
			CameraPosition = new AnimatedVector( parent: this );

			InternalChildren = new Drawable[] {
				Hitokori = new DrawableHitokori { Depth = -1 }.Center(),
				Tiles = HitObjectContainer.Center(),
				Judgements = new JudgementContainer<DrawableHitokoriJudgement>().Center()
			};

			if ( Auto = auto ) {
				AddInternal( AutoBot = new Bot {
					Position = new osuTK.Vector2( 100, -100 ),
					Anchor = Anchor.BottomLeft,
					Origin = Anchor.Centre
				} );
			}
		}

		public bool Auto = false;

		bool Started = false;
		HitokoriTile HeadTile;

		protected override void UpdateAfterChildren () {
			if ( !Started && HeadTile != null && Clock.CurrentTime >= HeadTile.Tile.Previous.LastPoint.HitTime ) {
				Started = true;

				Hitokori.Swap( HeadTile.Tile.Previous.LastPoint );
			}

			UpdateOffsets();
		}

		private void UpdateOffsets () {
			var all = Tiles.AliveObjects.OfType<IHasTilePosition>().Concat( Judgements ).Append( Hitokori );

			if ( FollowMode.Value == CameraFollowMode.Smooth ) {
				var averagePosition = ( all.Average( x => x.TilePosition ) + Hitokori.TilePosition ) / 2;
				CameraPosition.AnimateTo( averagePosition, CameraSpeed.Value );
			}

			foreach ( var tile in all ) {
				if ( tile is Drawable drawable ) drawable.Position = tile.TilePosition - CameraPosition;
			}
		}

		public override void Add ( DrawableHitObject hitObject ) {
			if ( hitObject is HitokoriTile tile ) {
				tile.Hitokori = Hitokori;

				if ( HeadTile is null ) {
					HeadTile = tile;
				}

				if ( Auto ) {
					tile.OnAutoClick += OnAutoClick;
				}

				tile.OnNewResult += OnTileResult;
				Tiles.Add( tile );
			} else {
				base.Add( hitObject );
			}
		}

		public override bool Remove ( DrawableHitObject h ) {
			if ( h is HitokoriTile tile ) {
				if ( Auto ) {
					tile.OnAutoClick -= OnAutoClick;
				}

				tile.OnNewResult -= OnTileResult;
				Tiles.Remove( tile );
				tile.RemoveNested();
			} else {
				base.Remove( h );
			}

			return true;
		}

		private void OnAutoClick ( HitokoriTile tile, AutoClickType type ) {
			switch ( type ) {
				case AutoClickType.Down:
					AutoBot.Hold();
					break;

				case AutoClickType.Up:
					AutoBot.Release();
					break;

				case AutoClickType.Press:
					AutoBot.Press();
					break;
			}
		}

		private void OnTileResult ( DrawableHitObject obj, JudgementResult result ) {
			if ( obj is HitokoriTile tile ) {
				tile.OnNewResult -= OnTileResult;

				if ( FollowMode.Value == CameraFollowMode.Dynamic ) {
					CameraPosition.AnimateTo( Hitokori.TilePosition, CameraSpeed.Value );
				}

				if ( tile.Tile.LastPoint.Duration > MinimumBreakTime || tile.Tile.IsLast ) {
					Hitokori.Contract();
					AutoBot?.AllowGhosting();

					if ( !tile.Tile.IsLast ) {
						this.Delay( tile.Tile.LastPoint.Duration - 1000 ).Schedule( () => {
							Hitokori.AnimateDistance();
							AutoBot?.ForbidGhosting();
						} );
					}
				}
			} else if ( obj is DrawableTilePoint point ) {
				Judgements.Add( new DrawableHitokoriJudgement( result, point ) );
			}
		}

		public static readonly double MinimumBreakTime = 1000;

		[BackgroundDependencyLoader]
		private void load ( HitokoriSettingsManager config ) {
			FollowMode = config.GetBindable<CameraFollowMode>( HitokoriSetting.CameraFollowMode );
			CameraSpeed = config.GetBindable<double>( HitokoriSetting.CameraSpeed );
		}
	}
}
