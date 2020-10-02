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
		List<Connector> LinesToMe = new List<Connector>();
		SpriteText Label;

		public TileMarker ( TilePoint tile, Color4 color, TickSize size = TickSize.Auto ) {
			Tile = tile;

			if ( size == TickSize.Auto ) {
				size = Tile.Size;
			}

			TickSize = size;

			AddInternal(
				Circle = new Circle {
					Width = (float)TickSize.Size(),
					Height = (float)TickSize.Size(),
					Colour = color,
					Alpha = 0
				}.Center()
			);

			this.Center();
		}

		public void ChangeTickSize ( TickSize size ) {
			Circle.Width = (float)size.Size();
			Circle.Height = (float)size.Size();

			TickSize = size;
		}

		public TileMarker ( TilePoint tile ) : this( tile, tile.Color, tile.Size ) { }

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

			return new[] { 700, ReverseMarker?.Appear(), lineDuration }.Max().Value;
		}

		public double Hit () {
			Circle.ScaleTo( new Vector2( 4 ), 300 )
				.FadeColour( Colour4.Green, 300 )
				.FadeOut( 300 );

			double lineDuration = LinesToMe.Select( line => line.Disappear() ).Append( 0 ).Max();
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
			ImportantMarker?.Disappear();

			return new[] { 700, ReverseMarker?.Disappear(), lineDuration }.Max().Value;
		}
		// I guess they never miss, h u h?

		// ----------------

		public void Reverse ( bool isClockwise ) {
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
			TileConnector line;
			AddInternal(
				line = new TileConnector( from, Tile ) {
					Position = from.TilePosition - Tile.TilePosition
				}
			);
			LinesToMe.Add( line );
		}

		public void ConnectFrom ( TilePoint from, TilePoint around ) {
			ArchedTileConnector line;

			var a = from.TilePosition - around.TilePosition;
			var b = Tile.TilePosition - around.TilePosition;

			var angle = Math.Acos( Vector2.Dot( a, b ) / a.Length / b.Length );

			AddInternal(
				line = new ArchedTileConnector( from, Tile, around, Tile.IsClockwise ? angle : -angle ) {
					Position = around.TilePosition - Tile.TilePosition
				}
			);
			LinesToMe.Add( line );
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
