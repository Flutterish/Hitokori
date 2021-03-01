using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using osuTK.Graphics;
using System;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class StandardOrbital : Orbital {
		private Circle circle;
		private Colour4 colour;
		public StandardOrbital ( IHasTilePosition parent, Radius radius, Colour4 colour ) : base( parent, radius ) {
			AddInternal(
				circle = new Circle {
					Width = HitokoriTile.SIZE * 2,
					Height = HitokoriTile.SIZE * 2,
				}.Center()
			);

			Colour = colour;

			RevokeImportant();
		}

		new public Color4 Colour {
			get => colour;
			set {
				colour = value;
				circle.Colour = value;
				Trail.Colour = value;
			}
		}

		protected override bool ComputeIsMaskedAway ( RectangleF maskingBounds )
			=> false;

		public override void MakeImportant () {
			circle.ScaleTo( 1.1f, 100 );
			circle.FadeTo( 1, 100 );
		}

		public override void RevokeImportant () {
			circle.ScaleTo( 0.9f, 100 );
			circle.FadeTo( 0.8f, 100 );
		}

		private bool isHolding;
		private double starTimer;
		private double starInterval = 40;
		private Random starRandomizer = new Random();
		[Resolved( canBeNull: true )]
		private SparklePool sparklePool { get; set; }
		public override void OnHold () {
			isHolding = true;
			ReleaseStars( 5 );
		}

		public override void OnPress () {
			ReleaseStars( 5 );
		}

		public override void OnRelease () {
			isHolding = false;
		}

		protected void ReleaseStars ( int count ) {
			if ( sparklePool is null || Playfield is null ) return;

			sparklePool.Clock = Clock;

			while ( count-- > 0 ) {
				var sparkle = sparklePool.Borrow( duration: 300 );

				sparkle.Colour = colour.Lighten( 0.3f );
				sparkle.Size = new Vector2( 20 );

				sparkle.gravity = new Vector2( 0, 2000 );
				sparkle.velocity = new Vector2( starRandomizer.Next( -300, 300 ), starRandomizer.Next( -500, 200 ) );
				sparkle.angularVelocity = starRandomizer.Next( -200, 200 );

				Playfield.SFX.Add( sparkle );
				sparkle.Position = ToSpaceOfOtherDrawable( Vector2.Zero, Playfield ) - Playfield.LayoutSize / 2;
			}
		}

		[Resolved( canBeNull: true )]
		private HitokoriPlayfield Playfield { get; set; }
		protected override void Update () {
			base.Update();

			if ( isHolding ) {
				starTimer += Clock.ElapsedFrameTime;

				int stars = (int)Math.Floor( starTimer / starInterval );
				starTimer -= stars * starInterval;
				ReleaseStars( stars );
			}
		}
	}
}
