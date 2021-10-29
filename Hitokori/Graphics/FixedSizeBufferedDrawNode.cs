///
/// This file is copied from osu-framework in order to work with our constraints.
/// This is a woraround of a Frame Buffer sizing issue which allows them to scale without bound.
///

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.OpenGL.Buffers;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osuTK;
using osuTK.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Graphics {
	public class FixedSizeBufferedDrawNode : TexturedShaderDrawNode {
		protected new IBufferedDrawableWithSizeConstraints Source => (IBufferedDrawableWithSizeConstraints)base.Source;

		/// <summary>
		/// The child <see cref="DrawNode"/> which is used to populate the <see cref="FrameBuffer"/>s with.
		/// </summary>
		[AllowNull]
		protected DrawNode Child { get; private set; }

		/// <summary>
		/// Data shared amongst all <see cref="BufferedDrawNode"/>s, providing storage for <see cref="FrameBuffer"/>s.
		/// </summary>
		protected readonly FixedSizeBufferedDrawNodeSharedData SharedData;

		/// <summary>
		/// Contains the colour and blending information of this <see cref="DrawNode"/>.
		/// </summary>
		protected new DrawColourInfo DrawColourInfo { get; private set; }

		protected RectangleF DrawRectangle { get; private set; }

		private Color4 backgroundColour;
		private RectangleF screenSpaceDrawRectangle;
		private Vector2 frameBufferScale;
		private Vector2 targetFrameBufferSize;
		private Vector2 frameBufferSize;

		private Vector2 maxFrameBufferSize;
		private bool allowResizing;

		public FixedSizeBufferedDrawNode ( IBufferedDrawableWithSizeConstraints source, DrawNode child, FixedSizeBufferedDrawNodeSharedData sharedData )
			: base( source ) {
			Child = child;
			SharedData = sharedData;
		}

		public override void ApplyState () {
			base.ApplyState();

			backgroundColour = Source.BackgroundColour;
			screenSpaceDrawRectangle = Source.ScreenSpaceDrawQuad.AABBFloat;
			DrawColourInfo = Source.FrameBufferDrawColour ?? new DrawColourInfo( Color4.White, base.DrawColourInfo.Blending );
			frameBufferScale = Source.FrameBufferScale;
			maxFrameBufferSize = Source.MaximumFrameBufferSize;
			allowResizing = Source.AllowFrameBufferResizing;

			targetFrameBufferSize = new Vector2(
				MathF.Ceiling( screenSpaceDrawRectangle.Width  * frameBufferScale.X ),
				MathF.Ceiling( screenSpaceDrawRectangle.Height * frameBufferScale.Y )
			);

			frameBufferSize = allowResizing ? new Vector2( 
				Math.Min( targetFrameBufferSize.X, maxFrameBufferSize.X ),
				Math.Min( targetFrameBufferSize.Y, maxFrameBufferSize.Y )
			) : maxFrameBufferSize;

			DrawRectangle = SharedData.PixelSnapping
				? new RectangleF( screenSpaceDrawRectangle.X, screenSpaceDrawRectangle.Y, targetFrameBufferSize.X, targetFrameBufferSize.Y )
				: screenSpaceDrawRectangle;

			Child.ApplyState();
		}

		/// <summary>
		/// Whether this <see cref="BufferedDrawNode"/> should be redrawn.
		/// </summary>
		protected bool RequiresRedraw => GetDrawVersion() > SharedData.DrawVersion;

		/// <summary>
		/// Retrieves the version of the state of this <see cref="DrawNode"/>.
		/// The <see cref="BufferedDrawNode"/> will only re-render if this version is greater than that of the rendered <see cref="FrameBuffer"/>s.
		/// </summary>
		/// <remarks>
		/// By default, the <see cref="BufferedDrawNode"/> is re-rendered with every <see cref="DrawNode"/> invalidation.
		/// </remarks>
		/// <returns>A version representing this <see cref="DrawNode"/>'s state.</returns>
		protected virtual long GetDrawVersion () => InvalidationID;

		public sealed override void Draw ( Action<TexturedVertex2D> vertexAction ) {
			if ( RequiresRedraw ) {
				//FrameStatistics.Increment( StatisticsCounterType.FBORedraw );

				SharedData.ResetCurrentEffectBuffer();

				using ( establishFrameBufferViewport() ) {
					// Fill the frame buffer with drawn children
					using ( BindFrameBuffer( SharedData.MainBuffer ) ) {
						// We need to draw children as if they were zero-based to the top-left of the texture.
						// We can do this by adding a translation component to our (orthogonal) projection matrix.
						GLWrapper.PushOrtho( screenSpaceDrawRectangle );
						GLWrapper.Clear( new ClearInfo( backgroundColour ) );

						Child.Draw( vertexAction );

						GLWrapper.PopOrtho();
					}

					PopulateContents();
				}

				SharedData.DrawVersion = GetDrawVersion();
			}

			Shader.Bind();

			base.Draw( vertexAction );
			DrawContents();

			Shader.Unbind();
		}

		/// <summary>
		/// Populates the contents of the effect buffers of <see cref="SharedData"/>.
		/// This is invoked after <see cref="Child"/> has been rendered to the main buffer.
		/// </summary>
		protected virtual void PopulateContents () {
		}

		/// <summary>
		/// Draws the applicable effect buffers of <see cref="SharedData"/> to the back buffer.
		/// </summary>
		protected virtual void DrawContents () {
			DrawFrameBuffer( SharedData.MainBuffer, DrawRectangle, DrawColourInfo.Colour );
		}

		/// <summary>
		/// Binds and initialises a <see cref="FrameBuffer"/> if required.
		/// </summary>
		/// <param name="frameBuffer">The <see cref="FrameBuffer"/> to bind.</param>
		/// <returns>A token that must be disposed upon finishing use of <paramref name="frameBuffer"/>.</returns>
		protected IDisposable BindFrameBuffer ( FrameBuffer frameBuffer ) {
			// This setter will also take care of allocating a texture of appropriate size within the frame buffer.
			frameBuffer.Size = frameBufferSize;

			frameBuffer.Bind();

			return new ValueInvokeOnDisposal<FrameBuffer>( frameBuffer, b => b.Unbind() );
		}

		private IDisposable establishFrameBufferViewport () {
			// Disable masking for generating the frame buffer since masking will be re-applied
			// when actually drawing later on anyways. This allows more information to be captured
			// in the frame buffer and helps with cached buffers being re-used.
			RectangleI screenSpaceMaskingRect = new RectangleI( (int)Math.Floor( screenSpaceDrawRectangle.X ), (int)Math.Floor( screenSpaceDrawRectangle.Y ), (int)frameBufferSize.X + 1, (int)frameBufferSize.Y + 1 );

			GLWrapper.PushMaskingInfo( new MaskingInfo {
				ScreenSpaceAABB = screenSpaceMaskingRect,
				MaskingRect = screenSpaceDrawRectangle,
				ToMaskingSpace = Matrix3.Identity,
				BlendRange = 1,
				AlphaExponent = 1,
			}, true );

			// Match viewport to FrameBuffer such that we don't draw unnecessary pixels.
			GLWrapper.PushViewport( new RectangleI( 0, 0, (int)frameBufferSize.X, (int)frameBufferSize.Y ) );
			GLWrapper.PushScissor( new RectangleI( 0, 0, (int)frameBufferSize.X, (int)frameBufferSize.Y ) );
			GLWrapper.PushScissorOffset( screenSpaceMaskingRect.Location );

			return new ValueInvokeOnDisposal<FixedSizeBufferedDrawNode>( this, d => d.returnViewport() );
		}

		private void returnViewport () {
			GLWrapper.PopScissorOffset();
			GLWrapper.PopViewport();
			GLWrapper.PopScissor();
			GLWrapper.PopMaskingInfo();
		}

		protected override void Dispose ( bool isDisposing ) {
			base.Dispose( isDisposing );

			Child?.Dispose();
			Child = null;
		}
	}
}
