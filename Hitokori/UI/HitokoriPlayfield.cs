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
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.UI {
	[Cached]
	public class HitokoriPlayfield : Playfield {
		private Dictionary<OrbitalGroup, TilePoint> paths = new();
		public readonly Container<Drawable> Everything;
		public const float DefaultPositionScale = 90 * 0.6f;
		[Cached]
		public readonly BeatProvider BeatProvider = new();

		OrbitalGroup addPath ( TilePoint firstTile ) {
			var orbitals = new OrbitalGroup( firstTile );
			paths.Add( orbitals, firstTile );
			Everything.Add( orbitals );
			return orbitals;
		}

		void removePath ( OrbitalGroup group ) {
			paths.Remove( group );
			group.Expire();
		}

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
				addPath( tile );
			}

			positionScale.BindValueChanged( _ => updateCameraViewport() );
		}

		private BindableFloat positionScale = new( DefaultPositionScale );
		private BindableBool doKiaiBeat = new( true );

		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );
			config?.BindWith( HitokoriSetting.DoKiaiBeat, doKiaiBeat );

			BeatProvider.OnBeat += OnBeat;
		}

		private void OnBeat ( int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes, BeatProvider provider ) {
			if ( effectPoint.KiaiMode ) {
				this.TransformBindableTo( kiaiScale, 1.1, 0 ).Then().TransformBindableTo( kiaiScale, 1, timingPoint.BeatLength * 1.2, Easing.Out );
			}
		}

		private Dictionary<HitObject, JudgementResult> results = new();
		public bool TryGetResultFor ( HitObject hitObject, [NotNullWhen(true)] out JudgementResult? result )
			=> results.TryGetValue( hitObject, out result ) && result.HasResult;

		protected override void LoadComplete () {
			base.LoadComplete();

			RegisterPool<SwapTilePoint, DrawableSwapTilePoint>( 30 );
			RegisterPool<PassThroughTilePoint, DrawableTapTilePoint>( 30 );
			RegisterPool<NoJudgementTilePoint, DrawableNoJudgementTilePoint>( 2 );

			NewResult += (dho, result) => {
				results.Add( dho.HitObject, result );
			};
			RevertResult += (dho, result) => {
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

		private void updateCameraViewport () {
			updateCamera();
			Scale = new Vector2( (float)( cameraScale.Value * ( doKiaiBeat.Value ? kiaiScale.Value : 1 ) ) / positionScale.Value );
			Everything.Position = -cameraMiddle.Value * positionScale.Value;
		}

		protected override void UpdateAfterChildren () {
			base.UpdateAfterChildren();

			updateCameraViewport();
		}

		private Bindable<Vector2> cameraMiddle = new();
		private BindableDouble cameraScale = new( 1 );
		private BindableDouble kiaiScale = new( 1 );
		private BindableDouble inflateScale = new( 1 );
		void updateCamera () {
			if ( path is not null ) {
				cameraMiddle.Value = path.Position.ValueAt( Time.Current );

				var maxInflate = paths.Keys.Select( x => x.NormalizedEnclosingCircleRadius * 1.2 ).Append( 0.5 ).Max();
				this.TransformBindableTo( inflateScale, maxInflate, 3000 ); // this still needs to be eased because it can change quickly
				maxInflate = inflateScale.Value;
				var size = path.Size.ValueAt( Time.Current ) + new Vector2( (float)maxInflate * 2 );

				double scale;
				if ( size.X / size.Y > DrawSize.X / DrawSize.Y ) {
					scale = DrawSize.X / size.X;
				}
				else {
					scale = DrawSize.Y / size.Y;
				}

				if ( double.IsFinite( scale ) ) {
					cameraScale.Value = scale / 2;
				}

				return;
			}
			else {
				var tiles = HitObjectContainer.AliveObjects.Select( x => x.HitObject ).OfType<TilePoint>();
				var points = tiles.Select( x => x.Position );

				if ( !points.Any() ) return;

				// we add interpolated points so the positioning is smooth rather than jumpy when a new hitobject spawns
				var p = tiles.Last();
				if ( p.ToNext is TilePointConnector next )
					points = points.Append( p.Position + ( next.To.Position - p.Position ) * Math.Clamp( ( Time.Current + 2000 - next.StartTime ) / next.Duration, 0, 1 ) );

				p = tiles.First();
				if ( p.FromPrevious is TilePointConnector prev )
					points = points.Append( prev.From.Position + ( p.Position - prev.From.Position ) * Math.Clamp( ( Time.Current + 2000 - prev.StartTime ) / prev.Duration, 0, 1 ) );

				var maxInflate = paths.Keys.Select( x => (double)x.NormalizedEnclosingCircleRadius * 1.2f ).Append( 0.5 ).Max();

				var boundingBox = new Box2d(
					points.Min( x => x.X ) - maxInflate,
					points.Min( x => x.Y ) - maxInflate,
					points.Max( x => x.X ) + maxInflate,
					points.Max( x => x.Y ) + maxInflate
				);

				this.TransformBindableTo( cameraMiddle, (Vector2)new Vector2d(
					( boundingBox.Left + boundingBox.Right ) / 2,
					( boundingBox.Top + boundingBox.Bottom ) / 2
				), 1000 );

				// this makes it so scaling doesnt go for "just enough", but rather keeps the current view and everything else in view
				// we do this after the positioning, so it doesnt affect it and creating a "dragging" effect
				boundingBox = new Box2d(
					Math.Min( boundingBox.Left, cameraMiddle.Value.X ),
					Math.Min( boundingBox.Top, cameraMiddle.Value.Y ),
					Math.Max( boundingBox.Right, cameraMiddle.Value.X ),
					Math.Max( boundingBox.Bottom, cameraMiddle.Value.Y )
				);

				double scale;
				if ( boundingBox.Width / boundingBox.Height > DrawSize.X / DrawSize.Y ) {
					scale = DrawSize.X / boundingBox.Width;
				}
				else {
					scale = DrawSize.Y / boundingBox.Height;
				}

				if ( !double.IsFinite( scale ) ) return;

				double speedup =
					cameraScale.Value > scale
					? cameraScale.Value / scale
					: 1;

				this.TransformBindableTo( cameraScale, scale / 2, 3000 / speedup );
			}
		}
	}
}
