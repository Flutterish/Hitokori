using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.UI.Visuals;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose.SelectionOverlays {
	public class TilePointOverlay : SelectionOverlay<TilePoint> {
		private TilePointVisualPiece piece;
		private Drawable pieceProxy;
		private bool proxyVisible = false;

		public TilePointOverlay ( TilePoint tp ) : base( tp ) {
			AddInternal( piece = new TilePointVisualPiece {
				Origin = Anchor.Centre,
				BorderColour = Colour4.Yellow,
				Colour = Colour4.Transparent,
				ShowShadows = false,
				OverlapConnectors = false
			} );

			piece.InAnimationProgress.Value = 1;
			piece.OutAnimationProgress.Value = 1;

			pieceProxy = piece.CreateProxy();
		}

		public override Quad ScreenSpaceDrawQuad => piece.ScreenSpaceDrawQuad;

		protected override void LoadComplete () {
			base.LoadComplete();

			piece.PositionScale.BindTo( Playfield.PositionScale );
		}

		public override void Show () {
			base.Show();

			if ( !proxyVisible ) {
				proxyVisible = true;
				Composer.ProxiedSelectionContainer.Add( pieceProxy );
			}
		}

		public override void Hide () {
			base.Hide();

			if ( proxyVisible ) {
				proxyVisible = false;
				Composer.ProxiedSelectionContainer.Remove( pieceProxy );
			}
		}

		protected override void Dispose ( bool isDisposing ) {
			base.Dispose( isDisposing );

			pieceProxy.Expire();
		}

		protected override void Update () {
			base.Update();

			piece.Position = PositionOf( HitObject );
			piece.Scale = Playfield.Scale;

			piece.AroundPosition.Value = HitObject.Position;
			piece.ToPosition.Value = HitObject.Next?.Position;
			piece.FromPosition.Value = HitObject.Previous?.Position;
		}
	}
}
