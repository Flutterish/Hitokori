using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
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

#nullable enable

namespace osu.Game.Rulesets.Hitokori.UI {
	[Cached]
	public class HitokoriPlayfield : Playfield {
		private Dictionary<OrbitalGroup, TilePoint> paths = new();
		public readonly Container Everything;
		public const float DefaultPositionScale = 90;

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

		public HitokoriPlayfield () {
			Origin = Anchor.Centre;
			Anchor = Anchor.Centre;

			AddInternal( Everything = new Container {
				AutoSizeAxes = Axes.Both,
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				Children = new Drawable[] {
					HitObjectContainer
				}
			} );
		}

		private BindableFloat positionScale = new( DefaultPositionScale );
		[BackgroundDependencyLoader( permitNulls: true )]
		private void load ( HitokoriConfigManager config ) {
			config?.BindWith( HitokoriSetting.PositionScale, positionScale );
		}

		private Dictionary<HitObject, JudgementResult> results = new();
		public bool TryGetResultFor ( HitObject hitObject, [NotNullWhen(true)] out JudgementResult? result )
			=> results.TryGetValue( hitObject, out result ) && result.HasResult;

		protected override void LoadComplete () {
			base.LoadComplete();

			RegisterPool<SwapTilePoint, DrawableSwapTilePoint>( 30 );
			RegisterPool<PassThroughTilePoint, DrawablePassThroughTilePoint>( 30 );
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
				Origin = Anchor.Centre,
				Anchor = Anchor.Centre
			};

			container.DrawableHitObjectAdded += dho => {
				if ( dho.HitObject is not TilePoint current ) return;

				if ( current.Previous is null )
					addPath( current );
			};

			return container;
		}
		private class MyHitObjectContainer : HitObjectContainer {
			public MyHitObjectContainer () {
				RelativeSizeAxes = Axes.None;
				AutoSizeAxes = Axes.Both;
			}

			protected override void AddDrawable ( HitObjectLifetimeEntry entry, DrawableHitObject drawable ) {
				base.AddDrawable( entry, drawable );

				DrawableHitObjectAdded?.Invoke( drawable );
			}

			public event Action<DrawableHitObject>? DrawableHitObjectAdded;
		}

		protected override HitObjectLifetimeEntry CreateLifetimeEntry ( HitObject hitObject )
			=> new HitokoriLifetimeEntry( hitObject );

		protected override void UpdateAfterChildren () {
			base.UpdateAfterChildren();

			updateCamera();
			Scale = new Vector2( (float)cameraScale.Value / positionScale.Value );
			Everything.Position = -cameraMiddle.Value * positionScale.Value;
		}

		private Bindable<Vector2> cameraMiddle = new();
		private BindableDouble cameraScale = new( 1 );
		void updateCamera () { // TODO this could be precomputed
			var points = HitObjectContainer.AliveObjects.Select( x => x.HitObject ).OfType<TilePoint>().Select( x => x.Position );

			if ( !points.Any() ) return;

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
