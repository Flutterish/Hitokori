///
/// This file is copied from osu-framework in order to work with our constraints.
/// This is a woraround of a Frame Buffer sizing issue which allows them to scale without bound.
///

using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.OpenGL.Buffers;
using osuTK.Graphics.ES30;
using System;
using System.Reflection;

namespace osu.Game.Rulesets.Hitokori.Graphics {
	public class FixedSizeBufferedDrawNodeSharedData : IDisposable {
        /// <summary>
        /// The version of drawn contents currently present in <see cref="MainBuffer"/> and <see cref="effectBuffers"/>.
        /// This should only be modified by <see cref="BufferedDrawNode"/>.
        /// </summary>
        internal long DrawVersion = -1;

        /// <summary>
        /// The <see cref="FrameBuffer"/> which contains the original version of the rendered <see cref="Drawable"/>.
        /// </summary>
        public FrameBuffer MainBuffer { get; }

        /// <summary>
        /// Whether the frame buffer position should be snapped to the nearest pixel when blitting.
        /// This amounts to setting the texture filtering mode to "nearest".
        /// </summary>
        public readonly bool PixelSnapping;

        /// <summary>
        /// A set of <see cref="FrameBuffer"/>s which are used in a ping-pong manner to render effects to.
        /// </summary>
        private readonly FrameBuffer[] effectBuffers;

        /// <summary>
        /// Creates a new <see cref="BufferedDrawNodeSharedData"/> with no effect buffers.
        /// </summary>
        public FixedSizeBufferedDrawNodeSharedData ( RenderbufferInternalFormat[] formats = null, bool pixelSnapping = false )
            : this( 0, formats, pixelSnapping ) {
        }

        /// <summary>
        /// Creates a new <see cref="BufferedDrawNodeSharedData"/> with a specific amount of effect buffers.
        /// </summary>
        /// <param name="effectBufferCount">The number of effect buffers.</param>
        /// <param name="formats">The render buffer formats to attach to each frame buffer.</param>
        /// <param name="pixelSnapping">Whether the frame buffer position should be snapped to the nearest pixel when blitting.
        /// This amounts to setting the texture filtering mode to "nearest".</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="effectBufferCount"/> is less than 0.</exception>
        public FixedSizeBufferedDrawNodeSharedData ( int effectBufferCount, RenderbufferInternalFormat[] formats = null, bool pixelSnapping = false ) {
            if ( effectBufferCount < 0 )
                throw new ArgumentOutOfRangeException( nameof( effectBufferCount ), "Must be positive." );

            PixelSnapping = pixelSnapping;
            All filterMode = pixelSnapping ? All.Nearest : All.Linear;

            MainBuffer = new FrameBuffer( formats, filterMode );
            effectBuffers = new FrameBuffer[ effectBufferCount ];

            for ( int i = 0; i < effectBufferCount; i++ )
                effectBuffers[ i ] = new FrameBuffer( formats, filterMode );
        }

        private int currentEffectBuffer = -1;

        /// <summary>
        /// The <see cref="FrameBuffer"/> which contains the most up-to-date drawn effect.
        /// </summary>
        public FrameBuffer CurrentEffectBuffer => currentEffectBuffer == -1 ? MainBuffer : effectBuffers[ currentEffectBuffer ];

        /// <summary>
        /// Retrieves the next <see cref="FrameBuffer"/> which effects can be rendered to.
        /// </summary>
        /// <exception cref="InvalidOperationException">If there are no available effect buffers.</exception>
        public FrameBuffer GetNextEffectBuffer () {
            if ( effectBuffers.Length == 0 )
                throw new InvalidOperationException( $"The {nameof( FixedSizeBufferedDrawNode )} requested an effect buffer, but none were available." );

            if ( ++currentEffectBuffer >= effectBuffers.Length )
                currentEffectBuffer = 0;
            return effectBuffers[ currentEffectBuffer ];
        }

        /// <summary>
        /// Resets <see cref="CurrentEffectBuffer"/>.
        /// This should only be called by <see cref="BufferedDrawNode"/>.
        /// </summary>
        internal void ResetCurrentEffectBuffer () => currentEffectBuffer = -1;

        public void Dispose () {
            Action dispose = () => Dispose( true );
            typeof( GLWrapper ).GetMethod( "ScheduleDisposal", BindingFlags.NonPublic | BindingFlags.Static )!.Invoke( null, new object[] { dispose } ); // :))))))) its internal
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose ( bool isDisposing ) {
            MainBuffer.Dispose();

            for ( int i = 0; i < effectBuffers.Length; i++ )
                effectBuffers[ i ].Dispose();
        }
    }
}
