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
			optimizeScale( newPath, reference: path );		// 15.5k -> 300
			optimizePosition( newPath, reference: path );	// 27k -> 6k -> 4.7k -> 500

			return newPath;
		}

		private void optimizePosition ( CameraPath path, CameraPath reference, double threshold = 0.01 ) {
			if ( reference.Position.Animations.Count == 0 ) return;

			void pass ( AnimatedValue<Vector2> target, AnimatedValue<Vector2> source, double threshold ) {
				target.Clear();

				var pos = source.Animations[ 0 ].Value.ValueAtProgress( 0 );
				var time = source.Animations[ 0 ].StartTime;
				target.Animate( time, pos );

				int slices = 5;
				for ( int i = 0; i < source.Animations.Count - 1; i++ ) {
					var a = source.Animations[ i ];
					var b = source.Animations[ i + 1 ];

					source.ValueAt( b.EndTime ); // forward to that time so we can get interrupts registered
					for ( int k = 0; k < slices; k++ ) {
						var t = ( 1.0 / ( slices - 1 ) ) * k;

						var aEnd = Math.Min( a.EndTime, a.Value.InterruptedTime );
						var bEnd = Math.Min( b.EndTime, b.Value.InterruptedTime );

						var aPos = a.Value.ValueAt( a.StartTime + ( aEnd - a.StartTime ) * t );
						var bPos = b.Value.ValueAt( b.StartTime + ( bEnd - b.StartTime ) * t );

						var nextPos = aPos + ( bPos - aPos ) * (float)t;
						var size = reference.Size.ValueAt( aEnd );
						var minDist = Math.Max( Math.Max( size.X, size.Y ), 1 ) * threshold;

						if ( ( nextPos - pos ).Length >= minDist ) {
							t = a.StartTime + ( bEnd - a.StartTime ) * t;
							target.Animate( time, pos = nextPos, t - time );

							time = t;
						}
					}
				}

				target.Animate( time, source.Animations[^1].Value.ValueAtProgress( 1 ), source.Animations[^1].EndTime - time );
			}

			pass( path.Position, reference.Position, threshold * 10 ); // 500
		}

		private void optimizeScale ( CameraPath path, CameraPath reference, double threshold = 0.01 ) {
			if ( reference.Size.Animations.Count == 0 ) return;

			void pass ( AnimatedValue<Vector2> target, AnimatedValue<Vector2> source, double threshold ) {
				target.Clear();

				var size = source.Animations[ 0 ].Value.ValueAtProgress( 0 );
				var time = source.Animations[ 0 ].StartTime;
				target.Animate( time, size );

				int slices = 5;
				for ( int i = 0; i < source.Animations.Count - 1; i++ ) {
					var a = source.Animations[ i ];
					var b = source.Animations[ i + 1 ];

					source.ValueAt( b.EndTime ); // forward to that time so we can get interrupts registered
					for ( int k = 0; k < slices; k++ ) {
						var t = ( 1.0 / ( slices - 1 ) ) * k;

						var aEnd = Math.Min( a.EndTime, a.Value.InterruptedTime );
						var bEnd = Math.Min( b.EndTime, b.Value.InterruptedTime );

						var aSize = a.Value.ValueAt( a.StartTime + ( aEnd - a.StartTime ) * t );
						var bSize = b.Value.ValueAt( b.StartTime + ( bEnd - b.StartTime ) * t );

						var nextSize = aSize + ( bSize - aSize ) * (float)t;

						if ( Math.Abs( nextSize.X - size.X ) > size.X * threshold || Math.Abs( nextSize.Y - size.Y ) > size.Y * threshold ) {
							t = a.StartTime + ( bEnd - a.StartTime ) * t;
							target.Animate( time, size = nextSize, t - time );

							time = t;
						}
					}
				}

				target.Animate( time, source.Animations[^1].Value.ValueAtProgress( 1 ), source.Animations[^1].EndTime - time );
			}

			pass( path.Size, reference.Size, threshold * 10 );
		}

		private void optimizeRotation ( CameraPath path, CameraPath reference, double threshold = 0.01 ) {
			for ( int i = 0; i < reference.Rotation.Animations.Count; i++ ) {
				var a = reference.Rotation.Animations[ i ];

				path.Rotation.Animate( a.StartTime, a.Value.ValueAtProgress( 1 ), a.Duration );
			}
		}
	}
}
