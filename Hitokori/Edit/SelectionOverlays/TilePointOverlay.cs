using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.UI.Visuals;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Edit.SelectionOverlays {
	public class TilePointOverlay : SelectionOverlay<TilePoint> {
		private TilePointVisualPiece piece;
		private BufferedContainer<TilePointVisualPiece> buffer;

		public TilePointOverlay ( TilePoint tp ) : base ( tp ) {
			AddInternal( buffer = new BufferedContainer<TilePointVisualPiece> { // ew
				Origin = Anchor.Centre,
				Child = piece = new TilePointVisualPiece {
					Origin = Anchor.Centre,
					BorderColour = Colour4.Yellow,
					Colour = Colour4.Transparent,
					ShowShadows = false,
					OverlapConnectors = false
				},
				Alpha = 0.4f
			} );

			piece.InAnimationProgress.Value = 1;
			piece.OutAnimationProgress.Value = 1;
		}

		public override Quad ScreenSpaceDrawQuad => buffer.ScreenSpaceDrawQuad;

		protected override void LoadComplete () {
			base.LoadComplete();

			piece.PositionScale.BindTo( Playfield.PositionScale );
		}

		protected override void Update () {
			base.Update();

			if ( piece.DrawSize.X > buffer.DrawSize.X || piece.DrawSize.Y > buffer.DrawSize.Y )
				buffer.Size = new Vector2( Math.Max( piece.DrawSize.X, buffer.DrawSize.X ), Math.Max( piece.DrawSize.Y, buffer.DrawSize.Y ) );

			buffer.Position = PositionOf( HitObject );
			buffer.Scale = Playfield.Scale;

			piece.AroundPosition.Value = HitObject.Position;
			piece.ToPosition.Value = HitObject.Next?.Position;
			piece.FromPosition.Value = HitObject.Previous?.Position;
		}
	}
}
