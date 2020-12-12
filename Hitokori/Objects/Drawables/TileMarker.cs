using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class TileMarker : Container {
		TilePoint Tile;

		TickSize TickSize => Tile.Size;

		ReverseMarker cachedReverseMarker = new ReverseMarker().Center();
		ImportantMarker cachedImportantMarker = new ImportantMarker().Center();
		LineTileConnector cachedLineToMe = new LineTileConnector();
		ArchedPathTileConnector cachedPathConnector = new ArchedPathTileConnector();
		SpriteText cachedLabel = new OsuSpriteText {
			Anchor = Anchor.BottomCentre,
			Origin = Anchor.TopCentre,
			Position = new Vector2( 0, 18 ),
			Scale = new Vector2( 0.8f )
		}; // NOTE with these, in theory i dont have to null check the actually used elements

		Circle Circle;
		ReverseMarker ReverseMarker;
		ImportantMarker ImportantMarker;
		LineTileConnector LineToMe; // TODO move these to the playfield
		public readonly Bindable<bool> showLabel = new();

		ArchedPathTileConnector pathConnector;
		SpriteText Label;

		public TileMarker () {
			this.Center();

			AddInternal(
				Circle = new Circle { Alpha = 0 }.Center()
			);

			showLabel.BindValueChanged( v => {
				if ( v.NewValue ) {
					Label?.FadeIn( 200 );
				}
				else {
					Label?.FadeOut( 200 );
				} // TODO this can be opaque if shown after hit/miss
			} );
		}

		public void Apply ( TilePoint tile ) {
			Tile = tile;

			Circle.Size = new Vector2( (float)TickSize.Size() );
			Circle.Colour = tile.Color;
			Circle.Alpha = 0;
		}
		public void Free () {
			RemoveAll( x => x != Circle );
			ClearTransforms( true );
			Circle.Alpha = 0;
			LineToMe = null;
			lastAnimation = null;
			ReverseMarker = null;
			ImportantMarker = null;
			pathConnector = null;
			Label = null;
		}

		Action lastAnimation;
		double lastAnimationTime;
		void playLastAnimation () {
			if ( lastAnimation is not null ) {
				ClearTransformsAfter( lastAnimationTime, true );
				using ( BeginAbsoluteSequence( lastAnimationTime, true ) ) {
					lastAnimation();
				}
			}
		}

		public void Appear () {
			lastAnimation = Appear;
			lastAnimationTime = TransformStartTime;

			Circle.FadeInFromZero( 700, Easing.Out );
			Circle.ScaleTo( 1.6f, 200, Easing.Out )
				.Then()
				.ScaleTo( 1, 300, Easing.In );

			ReverseMarker?.Spin();
			ImportantMarker?.Appear();
			ImportantMarker?.Spin();
			if ( showLabel.Value ) Label?.FadeInFromZero( 700 );

			LineToMe?.Appear();
			pathConnector?.Appear();
			ReverseMarker?.Appear();
		}

		public void Hit () {
			lastAnimation = Hit;
			lastAnimationTime = TransformStartTime;

			Circle.ScaleTo( new Vector2( 4 ), 300 )
				.FadeColour( Colour4.Green, 300 )
				.FadeOut( 300 );

			LineToMe?.Disappear();
			pathConnector?.Disappear();
			ImportantMarker?.Disappear();
			ReverseMarker?.Disappear();
			if ( showLabel.Value ) Label?.FadeOutFromOne( 300 );
		}
		// or
		public void Miss () {
			lastAnimation = Miss;
			lastAnimationTime = TransformStartTime;

			Circle.ScaleTo( new Vector2( 2 ), 700 )
				.FadeColour( Colour4.Red, 700 )
				.FadeOut( 700 );

			LineToMe?.Disappear();
			pathConnector?.Disappear();
			ImportantMarker?.Disappear();
			ReverseMarker?.Disappear();
			if ( showLabel.Value ) Label?.FadeOutFromOne( 300 );
		}
		// I guess they never miss, h u h?

		// ----------------

		public void Reverse ( bool isClockwise ) {
			AddInternal( ReverseMarker = cachedReverseMarker );
			ReverseMarker.Scale = new Vector2( ( 1 + (float)TickSize.Size() / HitokoriTile.SIZE ) / 2 ); // BUG with hold tiles these are sometimes big instead on regular
			ReverseMarker.SetClockwise( isClockwise );

			playLastAnimation();
		}

		public void MarkImportant () {
			AddInternal( ImportantMarker = cachedImportantMarker );
			ImportantMarker.SetTickSize( TickSize );

			playLastAnimation();
		}
		public void ConnectFrom ( TilePoint from ) {
			AddInternal( LineToMe = cachedLineToMe );
			LineToMe.Reset();
			LineToMe.From = from;
			LineToMe.To = Tile;
			LineToMe.Position = from.TilePosition - Tile.TilePosition;

			playLastAnimation();
		}

		public void ConnectFrom ( TilePoint from, TilePoint around ) {
			var a = from.TilePosition - around.TilePosition;
			var b = Tile.TilePosition - around.TilePosition;

			var angle = Math.Acos( Vector2.Dot( a, b ) / a.Length / b.Length );

			AddInternal( pathConnector = cachedPathConnector );
			pathConnector.Reset();
			pathConnector.From = from;
			pathConnector.To = Tile;
			pathConnector.Around = around;
			pathConnector.Angle = Tile.IsClockwise ? angle : -angle;
			pathConnector.Position = around.TilePosition - Tile.TilePosition;

			playLastAnimation();
		}

		public void AddLabel ( string text ) {
			AddInternal( Label = cachedLabel );
			Label.Alpha = 0;
			Label.Text = text;
			Label.Colour = Circle.Colour;
			playLastAnimation();
		}
	}

	public enum TickSize {
		Small,
		Regular,
		Big,
		Auto
	}

	public static class TickSizeMethods {
		public static double Size ( this TickSize self )
			=> self switch
			{
				TickSize.Small => HitokoriTile.SIZE * 0.5,
				TickSize.Regular => HitokoriTile.SIZE,
				TickSize.Big => HitokoriTile.SIZE * 2,
				_ => throw new NotImplementedException(),
			};
	}
}
