using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawableConnector : CompositeDrawable {
		TilePointConnector connector;
		Box box;

		public DrawableConnector ( TilePointConnector connector ) {
			this.connector = connector;
			Anchor = Anchor.Centre;
			Origin = Anchor.Centre;
			AutoSizeAxes = Axes.Y;

			AddInternal( box = new Box {
				Height = 8,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				RelativeSizeAxes = Axes.X,
				Colour = Colour4.Black
			} );

			AddInternal( box = new Box {
				Height = 3,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				RelativeSizeAxes = Axes.X
			} );
		}

		protected override void Update () {
			base.Update();

			Rotation = (float)connector.From.Position.AngleTo( connector.To.Position ).RadToDeg();
			var nextDur = ( connector.To.ToNext ?? connector ).Duration;
			if ( Time.Current > connector.EndTime + nextDur ) 
				Expire();
			else if ( Time.Current > connector.EndTime ) {
				var progress = Math.Clamp( ( connector.EndTime - Time.Current + nextDur ) / nextDur, 0, 1 );
				Width = (float)(progress * ( connector.From.Position - connector.To.Position ).Length * 100);
				Position = (Vector2)(connector.To.Position * 100 - progress / 2 * ( connector.To.Position - connector.From.Position ) * 100);
			}
			else {
				var progress = Math.Clamp( ( Time.Current - connector.StartTime + 2000 ) / connector.Duration, 0, 1 );
				Width = (float)(progress * ( connector.From.Position - connector.To.Position ).Length * 100);
				Position = (Vector2)(connector.From.Position * 100 + progress / 2 * ( connector.To.Position - connector.From.Position ) * 100);
			}
		}
	}
}
