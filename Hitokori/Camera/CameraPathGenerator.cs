using osu.Game.Rulesets.Hitokori.Collections;
using osuTK;
using System;

namespace osu.Game.Rulesets.Hitokori.Camera {
	public abstract class CameraPathGenerator {
		public abstract CameraPath GeneratePath ();

		/// <summary>
		/// Generates a path with <see cref="GeneratePath"/> and then eases it
		/// </summary>
		public CameraPath GenerateEasedPath () {
			var path = GeneratePath();
			var newPath = new CameraPath();

			optimizeRotation( newPath, reference: path );
			optimizeScale( newPath, reference: path );
			optimizePosition( newPath, reference: path );

			return newPath;
		}

		private void optimizePosition ( CameraPath path, CameraPath reference, double threshold = 0.01 ) {
			if ( reference.Position.Animations.Count == 0 ) return;

			void pass ( AnimatedValue<Vector2> target, AnimatedValue<Vector2> source, double threshold, int slices = 5 ) {
				target.Clear();

				var pos = source.Animations[ 0 ].Value.ValueAtProgress( 0 );
				var time = source.Animations[ 0 ].StartTime;
				target.Animate( time, pos );

				for ( int i = 0; i < source.Animations.Count; i++ ) {
					var anim = source.Animations[ i ];

					source.ValueAt( anim.EndTime ); // forward to that time so we can get interrupts registered
					var endTime = Math.Min( anim.EndTime, anim.Value.InterruptedTime );
					var duration = endTime - anim.StartTime;

					for ( int k = 0; k < slices; k++ ) {
						var t = ( 1.0 / ( slices - 1 ) ) * k;

						t = anim.StartTime + duration * t;
						var nextPos = anim.Value.ValueAt( t );

						var size = reference.Size.ValueAt( t );
						var minDist = Math.Max( size.X, size.Y ) * threshold;

						if ( ( nextPos - pos ).Length > minDist ) {
							target.Animate( time, pos = nextPos, t - time );

							time = t;
						}
					}
				}

				target.Animate( time, source.Animations[^1].Value.ValueAtProgress( 1 ), source.Animations[^1].EndTime - time );
			}

			pass( path.Position, reference.Position, threshold * 10 );
		}

		private void optimizeScale ( CameraPath path, CameraPath reference, double threshold = 0.01 ) {
			if ( reference.Size.Animations.Count == 0 ) return;

			void pass ( AnimatedValue<Vector2> target, AnimatedValue<Vector2> source, double threshold, int slices = 5 ) {
				target.Clear();

				var size = source.Animations[ 0 ].Value.ValueAtProgress( 0 );
				var time = source.Animations[ 0 ].StartTime;
				target.Animate( time, size );

				for ( int i = 0; i < source.Animations.Count; i++ ) {
					var anim = source.Animations[ i ];

					source.ValueAt( anim.EndTime ); // forward to that time so we can get interrupts registered
					var endTime = Math.Min( anim.EndTime, anim.Value.InterruptedTime );
					var duration = endTime - anim.StartTime;

					for ( int k = 0; k < slices; k++ ) {
						var t = ( 1.0 / ( slices - 1 ) ) * k;

						t = anim.StartTime + duration * t;
						var nextSize = anim.Value.ValueAt( t );

						if ( Math.Abs( nextSize.X - size.X ) > size.X * threshold || Math.Abs( nextSize.Y - size.Y ) > size.Y * threshold ) {
							target.Animate( time, size = nextSize, t - time );

							time = t;
						}
					}
				}

				target.Animate( time, source.Animations[^1].Value.ValueAtProgress( 1 ), source.Animations[^1].EndTime - time );
			}

			pass( path.Size, reference.Size, threshold * 8 );
		}

		private void optimizeRotation ( CameraPath path, CameraPath reference, double threshold = 0.01 ) {
			for ( int i = 0; i < reference.Rotation.Animations.Count; i++ ) {
				var a = reference.Rotation.Animations[ i ];

				path.Rotation.Animate( a.StartTime, a.Value.ValueAtProgress( 1 ), a.Duration );
			}
		}
	}
}
