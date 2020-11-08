using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails;
using osu.Game.Rulesets.Hitokori.Settings;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
	public class Radius : Container {
		BindableDouble Opacity = new();
		Bindable<DashStyle> BorderStyle = new();

		public void AnimateDistance ( double length, double duration, Easing easing = Easing.None ) {
			this.ResizeTo( (float)length * 2, duration, easing );
		}

		public double Length => Width / 2;

		protected override void Update () {
			BeforeUpdate?.Invoke();
			base.Update();

			Rotation += (float)( 20 * Clock.ElapsedFrameTime / 1000 );
		}

		private Action BeforeUpdate;

		void LoadStyle ( DashStyle style ) { // TODO make connectors more crisp
			ClearInternal();
			BeforeUpdate = null;

			if ( style == DashStyle.Solid ) {
				ArchedConnector Solid = new ArchedConnector( Math.PI * 2, 1 );
				Solid.AlwaysPresent = true;
				Solid.Around = Vector2.Zero;
				Solid.LineRadius = 1;
				Solid.Connect( 0 );
				AddInternal( Solid );

				BeforeUpdate = () => {
					Solid.From = Vector2.UnitX * (float)Length;
					Solid.To = Vector2.UnitX * (float)Length;
				};
			}
			else if ( style == DashStyle.Dashed ) { // BUG when switching back to this, not all dashes exist
				int count = 10;
				List<ArchedConnector> dashes = new List<ArchedConnector>();

				double angle = Math.PI * 2 / count;
				double angleSkip = angle / 4;
				for ( int i = 0; i < count; i++ ) {
					ArchedConnector Dash = new ArchedConnector( angle - angleSkip, 1 );
					Dash.AlwaysPresent = true;
					Dash.Around = Vector2.Zero;
					Dash.LineRadius = 1;
					Dash.Connect( 0 );
					AddInternal( Dash );
					dashes.Add( Dash );
				}

				BeforeUpdate = () => {
					for ( int i = 0; i < count; i++ ) {
						var theta = angle * i;
						dashes[ i ].From = new Vector2( (float)( Math.Cos( theta ) * Length ), (float)( Math.Sin( theta ) * Length ) );
						dashes[ i ].To = new Vector2( (float)( Math.Cos( theta + angle - angleSkip ) * Length ), (float)( Math.Sin( theta + angle - angleSkip ) * Length ) );
					}
				};
			}
			else if ( style == DashStyle.Dotted ) {
				int count = 20;
				List<ArchedConnector> dots = new List<ArchedConnector>();

				double angle = Math.PI * 2 / count;
				for ( int i = 0; i < count; i++ ) {
					ArchedConnector Dot = new ArchedConnector( Math.PI * 2, 1 );
					Dot.From = Vector2.One * 0.4f;
					Dot.To = Vector2.One * 0.4f;
					Dot.AlwaysPresent = true;
					Dot.Around = Vector2.Zero;
					Dot.LineRadius = 1;
					Dot.Connect( 0 );
					AddInternal( Dot );
					dots.Add( Dot );
				}

				BeforeUpdate = () => {
					for ( int i = 0; i < count; i++ ) {
						var theta = angle * i;
						dots[ i ].Position = new Vector2( (float)( Math.Cos( theta ) * Length ), (float)( Math.Sin( theta ) * Length ) );
					}
				};
			}
		}

		[BackgroundDependencyLoader]
		private void load ( HitokoriSettingsManager config ) {
			config.BindWith( HitokoriSetting.RingOpacity, Opacity );
			config.BindWith( HitokoriSetting.RingDashStyle, BorderStyle );

			BorderStyle.BindValueChanged( v => LoadStyle( v.NewValue ), true );
			Opacity.BindValueChanged( v => Alpha = (float)v.NewValue, true );
		}
	}
}
