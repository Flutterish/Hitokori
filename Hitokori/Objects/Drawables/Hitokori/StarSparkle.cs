using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Timing;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class StarSparkle : SpriteIcon {
		public double StartTime;
		public double Duration;

		public StarSparkle () {
			Icon = FontAwesome.Solid.Star;
		}

		public bool IsBorrowed
			=> Clock.CurrentTime <= StartTime + Duration && Clock.CurrentTime >= StartTime;

		public Vector2 velocity;
		public Vector2 gravity;
		public double angularVelocity;
		public void Reset () {
			velocity = Vector2.Zero;
			gravity = Vector2.Zero;
			Position = Vector2.Zero;
			angularVelocity = 0;

			this.FadeOutFromOne( Duration );
			this.Scale = Vector2.One;
			this.ScaleTo( 0.2f, Duration );
		}

		protected override void Update () {
			base.Update();

			velocity += gravity * (float)( Clock.ElapsedFrameTime / 1000 );
			Position += velocity * (float)( Clock.ElapsedFrameTime / 1000 );
			Rotation += (float)( angularVelocity * Clock.ElapsedFrameTime / 1000 );
		}
	}

	public class SparklePool : IDisposable {
		private List<StarSparkle> sparkles = new List<StarSparkle>();
		public IFrameBasedClock Clock;

		public StarSparkle Borrow ( double duration ) {
			foreach ( var sparkle in sparkles ) {
				if ( !sparkle.IsBorrowed ) {
					( sparkle.Parent as Container )?.Remove( sparkle );
					sparkle.StartTime = Clock.CurrentTime;
					sparkle.Duration = duration;
					sparkle.Clock = Clock;
					sparkle.Reset();

					return sparkle;
				}
			}

			var newSparkle = new StarSparkle {
				StartTime = Clock.CurrentTime,
				Duration = duration,
				Clock = Clock
			};

			newSparkle.Reset();

			sparkles.Add( newSparkle );

			return newSparkle;
		}

		public void Dispose () {
			foreach ( var sparkle in sparkles ) {
				sparkle?.Dispose();
			}
		}
	}
}
