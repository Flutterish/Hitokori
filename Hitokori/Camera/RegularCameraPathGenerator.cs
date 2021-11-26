using osu.Game.Beatmaps;
using osu.Game.Rulesets.Hitokori.Objects;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Hitokori.Camera
{
    public class RegularCameraPathGenerator : CameraPathGenerator
    {
        Beatmap<HitokoriHitObject> beatmap;

        public RegularCameraPathGenerator(Beatmap<HitokoriHitObject> beatmap)
        {
            this.beatmap = beatmap;
        }

        public override CameraPath GeneratePath()
        {
            var path = new CameraPath();
            var tiles = beatmap.HitObjects.OfType<TilePoint>().ToArray();

            if (tiles.Length == 0) return path;

            var aliveTiles = new Queue<TilePoint>(50);
            double time = startTime(tiles[0]);
            int i = 0;

            while (time < endTime(tiles[^1]) + 1000)
            {
                while (aliveTiles.TryPeek(out var lastTile) && endTime(lastTile) < time)
                {
                    aliveTiles.Dequeue();
                }
                while (i < tiles.Length && startTime(tiles[i]) <= time)
                {
                    aliveTiles.Enqueue(tiles[i++]);
                }

                generateEntry(path, time, aliveTiles);

                time += 1000 / 120;
            }

            return path;
        }

        private double startTime(TilePoint tp)
        {
            double lifetimeOffset = tp.LifetimeOffset;

            return tp.StartTime - lifetimeOffset;
        }

        private double endTime(TilePoint tp)
        {
            double duration = tp.ToNext is null ? 100 : tp.ToNext.Duration;

            return tp.StartTime + duration + 200;
        }

        private void generateEntry(CameraPath path, double time, IEnumerable<TilePoint> tiles)
        {
            if (!tiles.Any()) return;

            var points = tiles.Select(x => x.Position);

            // we add interpolated points so the positioning is smooth rather than jumpy when a new hitobject spawns
            var lastTile = tiles.Aggregate(tiles.First(), (a, b) => a.StartTime > b.StartTime ? a : b);
            double seekforward = 100;
            while (lastTile.ToNext is TilePointConnector toNext && startTime(lastTile) < time + seekforward)
            {
                if (toNext.Duration == 0)
                {
                    points = points.Append(toNext.To.Position);
                }
                else
                {
                    points = points.Append(lastTile.Position + (toNext.To.Position - lastTile.Position) * Math.Clamp((time + seekforward - lastTile.StartTime) / toNext.Duration, 0, 1));
                }

                lastTile = toNext.To;
            }

            // same with the past points
            var firstTile = tiles.Aggregate(tiles.First(), (a, b) => a.StartTime < b.StartTime ? a : b);
            double seekback = 500;
            while (firstTile.FromPrevious is TilePointConnector fromPrevious && endTime(firstTile) > time - seekback)
            {
                if (fromPrevious.Duration == 0)
                {
                    points = points.Prepend(fromPrevious.From.Position);
                }
                else
                {
                    points = points.Prepend(fromPrevious.From.Position + (firstTile.Position - fromPrevious.From.Position) * Math.Clamp(((time - seekback) - fromPrevious.StartTime) / fromPrevious.Duration, 0, 1));
                }

                firstTile = fromPrevious.From;
            }

            var boundingBox = points.CalculateBoundingBox();

            path.Position.Animate(time, (Vector2)new Vector2d(
                (boundingBox.Left + boundingBox.Right) / 2,
                (boundingBox.Top + boundingBox.Bottom) / 2
            ), 1000);

            path.Size.Animate(time, new Vector2((float)boundingBox.Width, (float)boundingBox.Height), 2000);
        }
    }
}
