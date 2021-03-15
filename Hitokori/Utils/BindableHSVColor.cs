using osu.Framework.Bindables;
using osuTK.Graphics;
using System;

namespace osu.Game.Rulesets.Hitokori.Utils {
	public class BindableHSVColor {
		public readonly Bindable<Color4> Color = new();
		public readonly Bindable<float> H = new();
		public readonly Bindable<float> S = new();
		public readonly Bindable<float> V = new();

		public BindableHSVColor ( Color4 defaultValue = default ) {
			Color.Default = defaultValue;
			Color.Value = defaultValue;
			recomputeHSV();
			H.Default = H.Value;
			S.Default = S.Value;
			V.Default = V.Value;

			Action<ValueChangedEvent<float>> action = _ => {
				if ( isLocked ) return;
				isLocked = true;
				recomputeColor();
				isLocked = false;
			};
			H.ValueChanged += action;
			S.ValueChanged += action;
			V.ValueChanged += action;

			Color.BindValueChanged( v => {
				if ( isLocked ) return;
				isLocked = true;
				recomputeHSV();
				isLocked = false;
			} );
		}

		void recomputeColor () {
			var ch = TextureGeneration.FromHSV( H.Value, S.Value, V.Value );
			Color.Value = new Color4( ch.R, ch.G, ch.B, 255 );
		}

		void recomputeHSV () {
			var goal = TextureGeneration.RGBToHSV( new osuTK.Vector3( Color.Value.R, Color.Value.G, Color.Value.B ) );
			H.Value = goal.X;
			S.Value = goal.Y;
			V.Value = goal.Z;
		}

		private bool isLocked;
	}
}
