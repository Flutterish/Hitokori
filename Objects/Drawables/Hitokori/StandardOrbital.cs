using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Hitokori.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
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
					Colour = colour
				}.Center()
			);

			this.colour = colour;

			Trail.Colour = colour;
			sparklePool = new SparklePool();

			RevokeImportant();
		}

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
		private SparklePool sparklePool;
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
			sparklePool.Clock = Clock;

			while ( count-- > 0 ) {
				var sparkle = sparklePool.Borrow( duration: 300 );

				sparkle.Colour = colour.Lighten( 0.3f );
				sparkle.Size = new osuTK.Vector2( 20 );

				sparkle.gravity = new osuTK.Vector2( 0, 2000 );
				sparkle.velocity = new osuTK.Vector2( starRandomizer.Next( -300, 300 ), starRandomizer.Next( -500, 200 ) );
				sparkle.angularVelocity = starRandomizer.Next( -200, 200 );

				Playfield.SFX.Add( sparkle );
				sparkle.Position = Playfield.Everything.ToParentSpace( Hitokori.Position + Position ) - Playfield.LayoutSize / 2;
			}
		}

		protected override void Dispose ( bool isDisposing ) {
			base.Dispose( isDisposing );
			sparklePool?.Dispose();
		}

		private DrawableHitokori Hitokori => Parent as DrawableHitokori; // TODO supply these properly, its ok for now as the hierarchy wont change soon
		private HitokoriPlayfield Playfield => Hitokori.Parent.Parent as HitokoriPlayfield;
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
