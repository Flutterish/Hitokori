using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class TileMarker : Container {
		TilePoint Tile;

		TickSize TickSize;

		Circle Circle;
		ReverseMarker ReverseMarker;
		ImportantMarker ImportantMarker;
		List<Connector> LinesToMe = new();
		ArchedPathTileConnector pathConnector;
		SpriteText Label;

		public TileMarker () {
			this.Center();

			AddInternal(
				Circle = new Circle { Alpha = 0 }.Center()
			);
		}

		public void Apply ( TilePoint tile ) {
			Tile = tile;

			TickSize = tile.Size;
			Circle.Size = new Vector2( (float)TickSize.Size() );
			Circle.Colour = tile.Color;
			Circle.Alpha = 0;
		}
		public void Free () {
			RemoveAll( x => x != Circle );
			ClearTransforms( true );
			Circle.Alpha = 0;
			LinesToMe.Clear();
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
			Label?.FadeInFromZero( 700 );

			foreach ( var line in LinesToMe )
				line.Appear();
			pathConnector?.Appear();
			ReverseMarker?.Appear();
		}

		public void Hit () {
			lastAnimation = Hit;
			lastAnimationTime = TransformStartTime;

			Circle.ScaleTo( new Vector2( 4 ), 300 )
				.FadeColour( Colour4.Green, 300 )
				.FadeOut( 300 );

			foreach ( var line in LinesToMe )
				line.Disappear();
			pathConnector?.Disappear();
			ImportantMarker?.Disappear();
			ReverseMarker?.Disappear();
			Label?.FadeOutFromOne( 300 );
		}
		// or
		public void Miss () {
			lastAnimation = Miss;
			lastAnimationTime = TransformStartTime;

			Circle.ScaleTo( new Vector2( 2 ), 700 )
				.FadeColour( Colour4.Red, 700 )
				.FadeOut( 700 );

			foreach ( var line in LinesToMe )
				line.Disappear();
			pathConnector?.Disappear();
			ImportantMarker?.Disappear();
			ReverseMarker?.Disappear();
		}
		// I guess they never miss, h u h?

		// ----------------

		public void Reverse ( bool isClockwise ) {
			AddInternal(
				ReverseMarker = new ReverseMarker( isClockwise ) { Scale = new Vector2( ( 1 + (float)TickSize.Size() / HitokoriTile.SIZE ) / 2 ) }.Center()
			);

			playLastAnimation();
		}

		public void MarkImportant () {
			AddInternal(
				ImportantMarker = new ImportantMarker( TickSize ).Center()
			);

			playLastAnimation();
		}
		public void ConnectFrom ( TilePoint from ) {
			LineTileConnector line;
			AddInternal(
				line = new LineTileConnector( from, Tile ) {
					Position = from.TilePosition - Tile.TilePosition
				}
			);
			LinesToMe.Add( line );

			playLastAnimation();
		}

		public void ConnectFrom ( TilePoint from, TilePoint around ) {
			ArchedPathTileConnector line;

			var a = from.TilePosition - around.TilePosition;
			var b = Tile.TilePosition - around.TilePosition;

			var angle = Math.Acos( Vector2.Dot( a, b ) / a.Length / b.Length );

			AddInternal(
				line = new ArchedPathTileConnector( from, Tile, around, Tile.IsClockwise ? angle : -angle ) {
					Position = around.TilePosition - Tile.TilePosition
				}
			);
			pathConnector = line;

			playLastAnimation();
		}

		public void AddLabel ( string text ) {
			AddInternal( Label = new SpriteText {
				Text = text,
				Colour = Circle.Colour,
				Anchor = Anchor.BottomCentre,
				Origin = Anchor.TopCentre,
				Position = new Vector2( 0, 18 ),
				Scale = new Vector2( 0.8f )
			} );

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
