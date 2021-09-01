using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.Drawables;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Hitokori.Orbitals;
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
		private Dictionary<OrbitalGroup, TilePoint> paths = new();
		public readonly Container Everything;

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
			HitObjectContainer.Origin = Anchor.Centre;
			HitObjectContainer.Anchor = Anchor.Centre;
		}

		protected override void LoadComplete () {
			base.LoadComplete();

			RegisterPool<SwapTilePoint, DrawableSwapTilePoint>( 30 );
			RegisterPool<PassThroughTilePoint, DrawablePassThroughTilePoint>( 30 );
			RegisterPool<NoJudgementTilePoint, DrawableNoJudgementTilePoint>( 2 );
		}

		protected override HitObjectContainer CreateHitObjectContainer () {
			var container = new MyHitObjectContainer();

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

			public event Action<DrawableHitObject> DrawableHitObjectAdded;
		}

		protected override HitObjectLifetimeEntry CreateLifetimeEntry ( HitObject hitObject )
			=> new HitokoriLifetimeEntry( hitObject );

		protected override void UpdateAfterChildren () {
			base.UpdateAfterChildren();

			updateCamera();
		}

		void updateCamera () { // TODO this could probably be precomputed
			var objects = ( HitObjectContainer.AliveObjects as IEnumerable<Drawable> ).Concat( paths.Keys.Where( x => x.Parent is not null ) );
			if ( !objects.Any() ) return;
			var first = objects.First(); // TODO inflate the bounding box by half hax orbitals size in all directions

			var boundingBox = objects.Skip(1).Aggregate( (min: first.Position - first.Size / 2, max: first.Position + first.Size / 2), ( bounds, obj ) => {
				return (
					new Vector2( Math.Min( obj.X - obj.Size.X / 2, bounds.min.X ), Math.Min( obj.Y - obj.Size.Y / 2, bounds.min.Y ) ),
					new Vector2( Math.Max( obj.X + obj.Size.X / 2, bounds.max.X ), Math.Max( obj.Y + obj.Size.Y / 2, bounds.max.Y ) )
				);
			} );

			var boundingBoxCentres = objects.Skip(1).Aggregate( (min: first.Position, max: first.Position), ( bounds, obj ) => {
				return (
					new Vector2( Math.Min( obj.X, bounds.min.X ), Math.Min( obj.Y, bounds.min.Y ) ),
					new Vector2( Math.Max( obj.X, bounds.max.X ), Math.Max( obj.Y, bounds.max.Y ) )
				);
			} );

			if ( HitObjectContainer.AliveObjects.LastOrDefault()?.HitObject is TilePoint tile && tile.ToNext is TilePointConnector connector ) {
				var pos = (Vector2)(connector.From.OrbitalState.PivotPosition + ( connector.To.OrbitalState.PivotPosition - connector.From.OrbitalState.PivotPosition ) * (float)Math.Clamp( ( Time.Current - connector.StartTime ) / connector.Duration, 0, 1 ));
				pos *= 100;
				boundingBox = (
					new Vector2( Math.Min( pos.X, boundingBox.min.X ), Math.Min( pos.Y, boundingBox.min.Y ) ),
					new Vector2( Math.Max( pos.X, boundingBox.max.X ), Math.Max( pos.Y, boundingBox.max.Y ) )
				);
				boundingBoxCentres = (
					new Vector2( Math.Min( pos.X, boundingBoxCentres.min.X ), Math.Min( pos.Y, boundingBoxCentres.min.Y ) ),
					new Vector2( Math.Max( pos.X, boundingBoxCentres.max.X ), Math.Max( pos.Y, boundingBoxCentres.max.Y ) )
				);
			}

			var middle = ( boundingBoxCentres.min + boundingBoxCentres.max ) / 2;
			var width = boundingBox.max.X - boundingBox.min.X;
			var height = boundingBox.max.Y - boundingBox.min.Y;
			float scale = 1;
			if ( width / height > DrawSize.X / DrawSize.Y ) {
				scale = DrawSize.X / width;
			}
			else {
				scale = DrawSize.Y / height;
			}

			if ( !float.IsFinite( scale ) ) return;

			this.ScaleTo( scale * 0.5f, 4000 );
			Everything.MoveTo( -middle, 1000 );
		}
	}
}
