using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using osuTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

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

		public void ChangeTickSize ( TickSize size ) {
			Circle.Width = (float)size.Size();
			Circle.Height = (float)size.Size();

			TickSize = size;
		}

		public TileMarker () {
			this.Center();

			AddInternal(
				Circle = new Circle {
					Alpha = 0
				}.Center()
			);
		}
		public void OnApply ( TilePoint tilePoint, Color4 color, TickSize size = TickSize.Auto ) {
			Tile = tilePoint;
			if ( size == TickSize.Auto ) {
				size = Tile.Size;
			}
			TickSize = size;

			Circle.Colour = color;
			Circle.Alpha = 0;
			Circle.Width = Circle.Height = (float)TickSize.Size();
		}
		public void OnApply ( TilePoint tilePoint ) => OnApply( tilePoint, tilePoint.Color, tilePoint.Size );
		public void OnFree () {
			ClearTransforms();
			Circle.Alpha = 0;
			TickSize = TickSize.Auto;
			Tile = null;
			RemoveAll( x => x != Circle );
			LinesToMe.Clear();
			ReverseMarker = null;
			ImportantMarker = null;
			pathConnector = null;
			Label = null;
			lastAnimation = null;
		}

		private Action lastAnimation;
		private double lastAnimationTime;

		public double Appear () {
			Circle.FadeInFromZero( 700, Easing.Out );
			Circle.ScaleTo( 1.6f, 200, Easing.Out )
				.Then()
				.ScaleTo( 1, 300, Easing.In );

			ReverseMarker?.Spin();
			ImportantMarker?.Appear();
			ImportantMarker?.Spin();
			Label?.FadeInFromZero( 700 );

			double lineDuration = LinesToMe.Select( line => line.Appear() ).Append( 0 ).Max();
			pathConnector?.Appear();

			return new[] { 700, ReverseMarker?.Appear(), lineDuration }.Max().Value;
		}

		public double Hit () {
			Circle.ScaleTo( new Vector2( 4 ), 300 )
				.FadeColour( Colour4.Green, 300 )
				.FadeOut( 300 );

			double lineDuration = LinesToMe.Select( line => line.Disappear() ).Append( 0 ).Max();
			pathConnector?.Disappear();
			ImportantMarker?.Disappear();
			Label?.FadeOutFromOne( 300 );

			return new[] { 300, ReverseMarker?.Disappear(), lineDuration }.Max().Value;
		}
		// or
		public double Miss () {
			Circle.ScaleTo( new Vector2( 2 ), 700 )
				.FadeColour( Colour4.Red, 700 )
				.FadeOut( 700 );

			double lineDuration = LinesToMe.Select( line => line.Disappear() ).Append( 0 ).Max();
			pathConnector?.Disappear();
			ImportantMarker?.Disappear();

			return new[] { 700, ReverseMarker?.Disappear(), lineDuration }.Max().Value;
		}
		// I guess they never miss, h u h?

		// ----------------

		private void updateAnimations () {
			if ( lastAnimation is not null ) {
				using ( BeginAbsoluteSequence( lastAnimationTime, true ) ) {
					lastAnimation();
				}
			}
		}

		public void Reverse ( bool isClockwise ) { // TODO pool all of these
			AddInternal(
				ReverseMarker = new ReverseMarker( isClockwise ) { Scale = new Vector2( ( 1 + (float)TickSize.Size() / HitokoriTile.SIZE ) / 2 ) }.Center()
			);
		}

		public void MarkImportant () {
			AddInternal(
				ImportantMarker = new ImportantMarker( TickSize ).Center()
			);
		}
		public void ConnectFrom ( TilePoint from ) {
			LineTileConnector line;
			AddInternal(
				line = new LineTileConnector( from, Tile ) {
					Position = from.TilePosition - Tile.TilePosition
				}
			);
			LinesToMe.Add( line );
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
			updateAnimations();
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
