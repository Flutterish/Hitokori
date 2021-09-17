using osu.Framework.Graphics.Textures;
using osuTK;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace osu.Game.Rulesets.Hitokori {
	public static class TextureGeneration {
		public static TextureUpload Generate ( int width, int height, Func<int, int, Rgba32> generator ) {
			Image<Rgba32> image = new Image<Rgba32>( width, height );
			for ( int y = 0; y < height; y++ ) {
				var span = image.GetPixelRowSpan( y );
				for ( int x = 0; x < width; x++ ) {
					span[ x ] = generator( x, height - y - 1 );
				}
			}
			return new TextureUpload( image );
		}

		public static Rgba32 FromHSV ( float hueDeg, float S, float V, float A = 1 ) {
			// https://www.rapidtables.com/convert/color/hsv-to-rgb.html
			hueDeg %= 360;
			if ( hueDeg < 0 ) hueDeg += 360;

			float C = V * S;
			float X = C * ( 1 - Math.Abs( ( hueDeg / 60 ) % 2 - 1 ) );
			float m = V - C;

			float r, g, b;
			(r,g,b) = hueDeg switch {
				< 60  => (C, X, 0f),
				< 120 => (X, C, 0),
				< 180 => (0, C, X),
				< 240 => (0, X, C),
				< 300 => (X, 0, C),
				_     => (C, 0, X)
			};

			return new Rgba32( r + m, g + m, b + m, A ); // NOTE this can be faster if we switch to working with bytes as soon as possible but might result in accuracy loss
		}

		public static Vector3 RGBToHSV ( Vector3 rgb ) {
			// https://www.rapidtables.com/convert/color/rgb-to-hsv.html
			float cmax = MathF.Max( MathF.Max( rgb.X, rgb.Y ), rgb.Z );
			float cmin = MathF.Min( MathF.Min( rgb.X, rgb.Y ), rgb.Z );
			float delta = cmax - cmin;

			float hue;
			if ( delta == 0 ) hue = 0;
			else if ( cmax == rgb.X ) hue = 60 * ( (rgb.Y-rgb.Z)/delta % 6 );
			else if ( cmax == rgb.Y ) hue = 60 * ( (rgb.Z-rgb.X)/delta + 2 );
			else hue = 60 * ( (rgb.X-rgb.Y)/delta + 4 );

			float saturation = cmax == 0 ? 0 : ( delta / cmax );
			return new Vector3( hue, saturation, cmax );
		}

		public static TextureUpload HSVBoxWithSetHue ( int width, int height, float hueDeg )
			=> Generate( width, height, (x,y) => FromHSV( hueDeg, (float)x / width, (float)y / height ) );

		public static TextureUpload HSVCircle ( int size, float innerRadius, float outerRadius, float saturation = 1, float value = 1, float fadeDistance = 0, int? discreteCount = null ) {
			if ( innerRadius > outerRadius ) {
				var temp = innerRadius;
				innerRadius = outerRadius;
				outerRadius = temp;
			}

			float half = size / 2;

			return Generate( size, size, (x,y) => {
				float angleDeg = MathF.Atan2( y-half, x-half ) / MathF.PI * 180;
				if ( discreteCount.HasValue ) {
					float step = 360 / discreteCount.Value;
					angleDeg = MathF.Round( angleDeg / step ) * step;
				}
				float distance = MathF.Sqrt( (x-half)*(x-half) + (y-half)*(y-half) );
				float alpha;
				if ( distance < innerRadius ) {
					alpha = fadeDistance == 0 ? 0 : MathF.Max( 0, 1 - ( innerRadius - distance ) / fadeDistance );
				}
				else if ( distance > outerRadius ) {
					alpha = fadeDistance == 0 ? 0 : MathF.Max( 0, 1 - ( distance - outerRadius ) / fadeDistance );
				}
				else {
					alpha = 1;
				}
				return FromHSV( angleDeg, saturation, value, alpha );
			} );
		}
	}
}
