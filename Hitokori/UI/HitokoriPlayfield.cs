using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Hitokori.Camera;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Drawables;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Hitokori.Orbitals;
using osu.Game.Rulesets.Hitokori.Settings;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.UI {
	[Cached]
	public class HitokoriPlayfield : Playfield {
		private Dictionary<OrbitalGroup, TilePoint> chains = new();
		private Dictionary<int, OrbitalGroup> chainsByID = new();

		public readonly Container<Drawable> Everything;
		public const float DefaultPositionScale = 90 * 0.6f;
		[Cached]
		public readonly BeatProvider BeatProvider = new();

		public IEnumerable<OrbitalGroup> Chains => chains.Keys;
		public OrbitalGroup ChainWithID ( int id ) => chainsByID[ id ];

		public OrbitalGroup AddChain ( TilePoint firstTile ) {
			var orbitals = new OrbitalGroup( firstTile );
			chains.Add( orbitals, firstTile );
			chainsByID.Add( firstTile.ChainID, orbitals );
			Everything.Add( orbitals );
			return orbitals;
		}

		public void RemoveChain ( OrbitalGroup group ) {
			chains.Remove( group );
			chainsByID.Remove( group.CurrentTile.ChainID );
			Everything.Remove( group );
			group.Dispose();
		}
		public void RemoveChain ( int id ) => RemoveChain( chainsByID[ id ] );

		CameraPath? path;
		public HitokoriPlayfield ( Beatmap<HitokoriHitObject> beatmap, CameraPath? path = null ) {
			Origin = Anchor.Centre;
			Anchor = Anchor.Centre;

			this.path = path;

			AddInternal( Everything = new TransformContainer {
				Anchor = Anchor.Centre,
				Children = new Drawable[] {
					HitObjectContainer
				}
			} );

			AddInternal( BeatProvider );
			foreach ( var tile in beatmap.HitObjects.OfType<TilePoint>().Where( x => x.Previous is null ) ) {
				AddChain( tile );
			}

			PositionScale.BindValueChanged( _ => UpdateCameraViewport( 0 ) );
		}

		public readonly BindableFloat PositionScale = new( DefaultPositionScale );
		private BindableBool doKiaiBeat = new( true );

		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, PositionScale );
			config?.BindWith( HitokoriSetting.DoKiaiBeat, doKiaiBeat );

			BeatProvider.OnBeat += OnBeat;
		}

		private void OnBeat ( int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes, BeatProvider provider ) {
			if ( effectPoint.KiaiMode ) {
				this.TransformBindableTo( kiaiScale, 1.1, 0 ).Then().TransformBindableTo( kiaiScale, 1, timingPoint.BeatLength * 1.2, Easing.Out );
			}
		}


		public struct TileJudgement {
			public double TimeAbsolute;
		}
		private Dictionary<HitObject, TileJudgement> results = new();
		public virtual bool TryGetResultFor ( HitObject hitObject, out TileJudgement result )
			=> results.TryGetValue( hitObject, out result );

		protected override void LoadComplete () {
			base.LoadComplete();

			RegisterPool<SwapTilePoint, DrawableSwapTilePoint>( 30 );
			RegisterPool<PassThroughTilePoint, DrawableTapTilePoint>( 30 );
			RegisterPool<NoJudgementTilePoint, DrawableNoJudgementTilePoint>( 2 );

			NewResult += ( dho, result ) => {
				results.Add( dho.HitObject, new TileJudgement {
					TimeAbsolute = result.TimeAbsolute
				} );
			};
			RevertResult += ( dho, result ) => {
				results.Remove( dho.HitObject );
			};
		}

		protected override HitObjectContainer CreateHitObjectContainer () {
			var container = new MyHitObjectContainer() {
				Anchor = Anchor.Centre
			};

			return container;
		}
		private class MyHitObjectContainer : HitObjectContainer {
			public MyHitObjectContainer () {
				RelativeSizeAxes = Axes.None;
				AutoSizeAxes = Axes.None;
				Size = Vector2.Zero;
			}

			protected override void AddDrawable ( HitObjectLifetimeEntry entry, DrawableHitObject drawable ) {
				base.AddDrawable( entry, drawable );

				DrawableHitObjectAdded?.Invoke( drawable );
			}

			protected override void Update () {
				base.Update();
			}

			protected override bool ComputeIsMaskedAway ( RectangleF maskingBounds )
				=> false;

			public event Action<DrawableHitObject>? DrawableHitObjectAdded;
		}

		protected override HitObjectLifetimeEntry CreateLifetimeEntry ( HitObject hitObject )
			=> new HitokoriLifetimeEntry( hitObject );

		/// <summary>
		/// Should the camera be updated automatically or externaly?
		/// </summary>
		public bool ShouldUpdateCamera = true;
		public void UpdateCameraViewport ( double delta ) {
			if ( ShouldUpdateCamera ) {
				UpdateCamera( delta );

				if ( delta < 0 ) {
					delta = -delta;
				}

				var maxInflate = Chains.Select( x => x.NormalizedEnclosingCircleRadius * 1.2 ).Append( 0.5 ).Max();
				inflateScale.Value = inflateScale.Value + ( maxInflate - inflateScale.Value ) * (float)Math.Clamp( delta / 3000, 0, 1 ); // this still needs to be eased because it can change quickly
				maxInflate = inflateScale.Value;

				var size = CameraSize.Value + new Vector2( (float)maxInflate * 2 );

				double scale;
				if ( size.X / size.Y > DrawSize.X / DrawSize.Y ) {
					scale = DrawSize.X / size.X;
				}
				else {
					scale = DrawSize.Y / size.Y;
				}

				if ( double.IsFinite( scale ) ) {
					CameraScale.Value = scale / 2;
				}

				Scale = new Vector2( (float)( CameraScale.Value * ( doKiaiBeat.Value ? kiaiScale.Value : 1 ) ) / PositionScale.Value );
				Everything.Position = -CameraMiddle.Value * PositionScale.Value;
			}
			else {
				Scale = new Vector2( (float)CameraScale.Value / PositionScale.Value );
				Everything.Position = -CameraMiddle.Value * PositionScale.Value;

				CameraSize.Value = (DrawSize / 2 / (float)CameraScale.Value) - (Vector2.One * (float)inflateScale.Value * 2);
			}
		}

		protected override void UpdateAfterChildren () {
			base.UpdateAfterChildren();

			UpdateCameraViewport( Time.Elapsed );
		}

		public readonly Bindable<Vector2> CameraMiddle = new();
		protected readonly Bindable<Vector2> CameraSize = new();
		public readonly BindableDouble CameraScale = new( 1 );

		private BindableDouble kiaiScale = new( 1 );
		private BindableDouble inflateScale = new( 1 );
		protected virtual void UpdateCamera ( double delta ) {
			var state = GetCameraState();
			
			if ( path is not null ) {
				CameraMiddle.Value = state.Center;
				CameraSize.Value = state.Size;
			}
			else {
				if ( delta < 0 ) {
					delta = -delta;
				}

				CameraMiddle.Value = CameraMiddle.Value + ( state.Center - CameraMiddle.Value ) * (float)Math.Clamp( delta / 1000, 0, 1 );
				CameraSize.Value = CameraSize.Value + ( state.Size - CameraSize.Value ) * (float)Math.Clamp( delta / 2500, 0, 1 );
			}
		}

		protected virtual CameraState GetCameraState () {
			if ( path is not null )
				return path.StateAt( Time.Current );
			else {
				return RegularCameraPathGenerator.GenerateCameraState( Time.Current, HitObjectContainer.AliveObjects.Select( x => x.HitObject ).OfType<TilePoint>() ) ?? new CameraState {
					Center = CameraMiddle.Value,
					Size = CameraSize.Value,
					Rotation = 0
				};
			}
		}

		public Vector2 ScreenSpacePositionOf ( Vector2 normalizedPosition ) {
			return HitObjectContainer.ToScreenSpace( normalizedPosition * PositionScale.Value );
		}

		public Vector2 NormalizedPositionAtScreenSpace ( Vector2 screenSpace ) {
			return HitObjectContainer.ToLocalSpace( screenSpace ) / PositionScale.Value;
		}
	}
}
