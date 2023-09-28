using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Trails;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Hitokori.Utils;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.Hitokori {
    public class Radius : Container
    {
		BindableDouble Opacity = new( 0.15 );
		Bindable<DashStyle> BorderStyle = new( DashStyle.Dashed );

		public void AnimateDistance ( double length, double duration, Easing easing = Easing.None ) {
			this.ResizeTo( (float)length * 2, duration, easing );
		}

		public double Length => DrawWidth / 2;

		protected override void Update () {
			BeforeUpdate?.Invoke();
			base.Update();

			Rotation += (float)( 20 * Clock.ElapsedFrameTime / 1000 );
		}

		private Action BeforeUpdate;

		void LoadStyle ( DashStyle style ) {
			ClearInternal();
			BeforeUpdate = null;

			if ( style == DashStyle.Solid ) {
				CircularConnector Solid = new( Vector2.Zero, Vector2.Zero, Math.PI * 2 );
				Solid.AlwaysPresent = true;
				Solid.TrailRadius = 1;
				Solid.Connect( 0 );
				AddInternal( Solid.Center() );

				BeforeUpdate = () => {
					Solid.From = Vector2.UnitX * (float)Length;
					Solid.Position = Solid.From;
				};
			}
			else if ( style == DashStyle.Dashed ) {
				int count = 10;
				List<CircularConnector> dashes = new();

				double angle = Math.PI * 2 / count;
				double angleSkip = angle / 4;
				for ( int i = 0; i < count; i++ ) {
					CircularConnector Dash = new( Vector2.Zero, Vector2.Zero, angle - angleSkip );
					Dash.AlwaysPresent = true;
					Dash.TrailRadius = 1;
					Dash.Connect( 0 );
					AddInternal( Dash.Center() );
					dashes.Add( Dash );
				}

				BeforeUpdate = () => {
					for ( int i = 0; i < count; i++ ) {
						var theta = angle * i;
						dashes[ i ].From = new Vector2( (float)( Math.Cos( theta ) * Length ), (float)( Math.Sin( theta ) * Length ) );
						dashes[ i ].Position = dashes[ i ].From - new Vector2( dashes[ i ].TrailRadius );
					}
				};
			}
			else if ( style == DashStyle.Dotted ) {
				int count = 20;
				List<CircularConnector> dots = new List<CircularConnector>();

				double angle = Math.PI * 2 / count;
				for ( int i = 0; i < count; i++ ) {
					CircularConnector Dot = new( Vector2.One * 0.4f, Vector2.Zero, Math.PI * 2 );
					Dot.AlwaysPresent = true;
					Dot.TrailRadius = 1;
					Dot.Connect( 0 );
					AddInternal( Dot.Center() );
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

		[BackgroundDependencyLoader( true )]
		private void load ( HitokoriSettingsManager config ) {
			config?.BindWith( HitokoriSetting.RingOpacity, Opacity );
			config?.BindWith( HitokoriSetting.RingDashStyle, BorderStyle );

			BorderStyle.BindValueChanged( v => LoadStyle( v.NewValue ), true );
			Opacity.BindValueChanged( v => Alpha = (float)v.NewValue, true );
		}
	}
}
