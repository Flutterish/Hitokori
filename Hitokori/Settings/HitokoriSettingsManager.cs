using osu.Framework.Bindables;
using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Hitokori.Settings {
	public class HitokoriSettingsManager : RulesetConfigManager<HitokoriSetting> {
		public HitokoriSettingsManager ( SettingsStore settings, RulesetInfo ruleset, int? variant = null ) : base( settings, ruleset, variant ) { }

		protected override void InitialiseDefaults () {
			SetDefault( HitokoriSetting.CameraSpeed, 1, 1 / 2d, 3, 0.1 );
			SetDefault( HitokoriSetting.RingOpacity, 0.15, 0, 1, 0.01 );
			SetDefault( HitokoriSetting.RingDashStyle, DashStyle.Dashed );
			SetDefault( HitokoriSetting.ConnectorWidth, 1, 0.4, 4 );
			SetDefault( HitokoriSetting.HoldConnectorWidth, 1, 0.2, 2 );
			SetDefault( HitokoriSetting.ShowSpeeedChange, true );

			SetDefault( HitokoriSetting._HiColorR, 1f, 0, 1 );
			SetDefault( HitokoriSetting._HiColorG, 0f, 0, 1 );
			SetDefault( HitokoriSetting._HiColorB, 0f, 0, 1 );
			SetDefault( HitokoriSetting._KoriColorR, 0f, 0, 1 );
			SetDefault( HitokoriSetting._KoriColorG, 0f, 0, 1 );
			SetDefault( HitokoriSetting._KoriColorB, 1f, 0, 1 );
			SetDefault( HitokoriSetting._GreenBitchColorR, 0f, 0, 1 );
			SetDefault( HitokoriSetting._GreenBitchColorG, Color4.Green.G, 0, 1 );
			SetDefault( HitokoriSetting._GreenBitchColorB, 0f, 0, 1 );

			GetOriginalBindable<float>( HitokoriSetting._HiColorR ).ValueChanged += v => HiColor.Value = new Color4( v.NewValue, HiColor.Value.G, HiColor.Value.B, 1 );
			GetOriginalBindable<float>( HitokoriSetting._HiColorG ).ValueChanged += v => HiColor.Value = new Color4( HiColor.Value.R, v.NewValue, HiColor.Value.B, 1 );
			GetOriginalBindable<float>( HitokoriSetting._HiColorB ).ValueChanged += v => HiColor.Value = new Color4( HiColor.Value.R, HiColor.Value.G, v.NewValue, 1 );
			GetOriginalBindable<float>( HitokoriSetting._KoriColorR ).ValueChanged += v => KoriColor.Value = new Color4( v.NewValue, KoriColor.Value.G, KoriColor.Value.B, 1 );
			GetOriginalBindable<float>( HitokoriSetting._KoriColorG ).ValueChanged += v => KoriColor.Value = new Color4( KoriColor.Value.R, v.NewValue, KoriColor.Value.B, 1 );
			GetOriginalBindable<float>( HitokoriSetting._KoriColorB ).ValueChanged += v => KoriColor.Value = new Color4( KoriColor.Value.R, KoriColor.Value.G, v.NewValue, 1 );
			GetOriginalBindable<float>( HitokoriSetting._GreenBitchColorR ).ValueChanged += v => GreenBitchColor.Value = new Color4( v.NewValue, GreenBitchColor.Value.G, GreenBitchColor.Value.B, 1 );
			GetOriginalBindable<float>( HitokoriSetting._GreenBitchColorG ).ValueChanged += v => GreenBitchColor.Value = new Color4( GreenBitchColor.Value.R, v.NewValue, GreenBitchColor.Value.B, 1 );
			GetOriginalBindable<float>( HitokoriSetting._GreenBitchColorB ).ValueChanged += v => GreenBitchColor.Value = new Color4( GreenBitchColor.Value.R, GreenBitchColor.Value.G, v.NewValue, 1 );

			bool hilock = false;
			HiColor.ValueChanged += v => {
				if ( hilock ) return;
				hilock = true;
				GetOriginalBindable<float>( HitokoriSetting._HiColorR ).Value = v.NewValue.R;
				GetOriginalBindable<float>( HitokoriSetting._HiColorG ).Value = v.NewValue.G;
				GetOriginalBindable<float>( HitokoriSetting._HiColorB ).Value = v.NewValue.B;
				hilock = false;
			};
			bool korilock = false;
			KoriColor.ValueChanged += v => {
				if ( korilock ) return;
				korilock = true;
				GetOriginalBindable<float>( HitokoriSetting._KoriColorR ).Value = v.NewValue.R;
				GetOriginalBindable<float>( HitokoriSetting._KoriColorG ).Value = v.NewValue.G;
				GetOriginalBindable<float>( HitokoriSetting._KoriColorB ).Value = v.NewValue.B;
				korilock = false;
			};
			bool glock = false;
			GreenBitchColor.ValueChanged += v => {
				if ( glock ) return;
				glock = true;
				GetOriginalBindable<float>( HitokoriSetting._GreenBitchColorR ).Value = v.NewValue.R;
				GetOriginalBindable<float>( HitokoriSetting._GreenBitchColorG ).Value = v.NewValue.G;
				GetOriginalBindable<float>( HitokoriSetting._GreenBitchColorB ).Value = v.NewValue.B;
				glock = false;
			};

			base.InitialiseDefaults();

			HiColor.Value = new Color4(
				Get<float>( HitokoriSetting._HiColorR ),
				Get<float>( HitokoriSetting._HiColorG ),
				Get<float>( HitokoriSetting._HiColorB ),
				1
			);
			KoriColor.Value = new Color4(
				Get<float>( HitokoriSetting._KoriColorR ),
				Get<float>( HitokoriSetting._KoriColorG ),
				Get<float>( HitokoriSetting._KoriColorB ),
				1
			);
			GreenBitchColor.Value = new Color4(
				Get<float>( HitokoriSetting._GreenBitchColorR ),
				Get<float>( HitokoriSetting._GreenBitchColorG ),
				Get<float>( HitokoriSetting._GreenBitchColorB ),
				1
			);
		}

		// osu cant save colors yet
		private Bindable<Color4> HiColor = new( Color4.Red );
		private Bindable<Color4> KoriColor = new( Color4.Blue );
		private Bindable<Color4> GreenBitchColor = new( Color4.Green );

		new public Bindable<TValue> GetBindable<TValue> ( HitokoriSetting lookup ) {
			if ( lookup == HitokoriSetting.HiColor ) {
				var b = new Bindable<TValue>();
				b.BindTo( HiColor as Bindable<TValue> );
				return b;
			}
			else if ( lookup == HitokoriSetting.KoriColor ) {
				var b = new Bindable<TValue>();
				b.BindTo( KoriColor as Bindable<TValue> );
				return b;
			}
			else if ( lookup == HitokoriSetting.GreenBitchColor ) {
				var b = new Bindable<TValue>();
				b.BindTo( GreenBitchColor as Bindable<TValue> );
				return b;
			}
			else return base.GetBindable<TValue>( lookup );
		}

		new public void BindWith<TValue> ( HitokoriSetting lookup, Bindable<TValue> bindable ) {
			if ( lookup == HitokoriSetting.HiColor )
				bindable.BindTo( HiColor as Bindable<TValue> );
			else if ( lookup == HitokoriSetting.KoriColor )
				bindable.BindTo( KoriColor as Bindable<TValue> );
			else if ( lookup == HitokoriSetting.GreenBitchColor )
				bindable.BindTo( GreenBitchColor as Bindable<TValue> );
			else
				base.BindWith( lookup, bindable );
		}
	}
}
