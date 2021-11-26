﻿using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Collections
{
    /// <summary>
    /// A timeline where entries have duration. It keeps track of current time and rises events as entries are passed.
    /// </summary>
    public class TimelineSeeker<T> : Timeline<T, TimelineSeeker<T>.Entry>
    {
        private double currentTime;
        private Timeline<TimelineSeeker<T>.Entry> ends;
        private Dictionary<Entry, TimelineEntry<Entry>> endEntries = new();

        public double CurrentTime
        {
            get => currentTime;
            set => SeekTo(value);
        }

        public TimelineModifiedBehaviour ModifiedBehaviour = TimelineModifiedBehaviour.None;

        public TimelineSeeker(IComparer<T>? comparer = null) : base(comparer)
        {
            ends = new Timeline<Entry>(Comparer<Entry>.Create((a, b) => Comparer.Compare(a.Value, b.Value)));
        }

        public int Add(double startTime, double duration, T value)
            => Add(new Entry(startTime, duration, value));

        public override int Add(Entry entry)
        {
            if (entry.Duration < 0)
            {
                throw new InvalidOperationException($"Can not add an event with negative duration to a {nameof(TimelineSeeker<T>)} ( attempted to add an event with duration {entry.Duration} )");
            }

            if (ModifiedBehaviour is TimelineModifiedBehaviour.Replay && entry.StartTime <= currentTime)
            {
                var time = currentTime;
                seekBackwardToBefore(entry.StartTime);
                endEntries.Add(entry, ends[ends.Add(entry.EndTime, entry)]);
                var index = base.Add(entry);
                seekForwardToAfter(time);

                return index;
            }
            else
            {
                endEntries.Add(entry, ends[ends.Add(entry.EndTime, entry)]);
                return base.Add(entry);
            }
        }

        public override int Remove(Entry entry)
        {
            if (ModifiedBehaviour is TimelineModifiedBehaviour.Replay && entry.StartTime <= currentTime)
            {
                var time = currentTime;
                seekBackwardToBefore(entry.StartTime);
                if (endEntries.Remove(entry, out var v))
                    ends.Remove(v);
                var index = base.Remove(entry);
                seekForwardToAfter(time);

                return index;
            }
            else
            {
                if (endEntries.Remove(entry, out var v))
                    ends.Remove(v);
                return base.Remove(entry);
            }
        }

        public void SeekTo(double time)
        {
            if (time > currentTime)
            {
                seekForwardTo(time);
            }
            else if (time < currentTime)
            {
                seekBackwardTo(time);
            }
        }

        private void seekForwardTo(double time)
        {
            int nextStartIndex = FirstAfter(currentTime); // TODO these indices can be kept track of to eliminate the log_2(n) overhead
            int nextEndIndex = ends.FirstAfterOrAt(currentTime);

            while (true)
            {
                if (nextEndIndex >= ends.Count) nextEndIndex = -1;
                if (nextStartIndex >= Count) nextStartIndex = -1;

                if (nextStartIndex == -1)
                {
                    if (nextEndIndex == -1)
                    {
                        break;
                    }
                    else
                    {
                        var nextEnd = ends[nextEndIndex].Value;

                        if (nextEnd.EndTime < time)
                        {
                            EventEnded?.Invoke(nextEnd);
                            nextEndIndex++;
                        }
                        else break;
                    }
                }
                else if (nextEndIndex == -1)
                {
                    var nextStart = this[nextStartIndex];

                    if (nextStart.StartTime <= time)
                    {
                        EventStarted?.Invoke(nextStart);
                        nextStartIndex++;
                    }
                    else break;
                }
                else
                {
                    var nextStart = this[nextStartIndex];
                    var nextEnd = ends[nextEndIndex].Value;

                    // first end, then start
                    if (nextEnd.EndTime <= nextStart.StartTime)
                    {
                        if (nextEnd.EndTime < time)
                        {
                            EventEnded?.Invoke(nextEnd);
                            nextEndIndex++;
                        }
                        else if (nextStart.StartTime <= time)
                        {
                            EventStarted?.Invoke(nextStart);
                            nextStartIndex++;
                        }
                        else break;
                    }
                    else
                    {
                        if (nextStart.StartTime <= time)
                        {
                            EventStarted?.Invoke(nextStart);
                            nextStartIndex++;
                        }
                        else break;
                    }
                }
            }

            currentTime = time;
        }

        private void seekBackwardTo(double time)
        {
            int previousStartIndex = LastAtOrFirstBefore(currentTime);
            int previousEndIndex = ends.FirstBefore(currentTime);

            while (true)
            {
                if (previousStartIndex == -1)
                {
                    if (previousEndIndex == -1)
                    {
                        break;
                    }
                    else
                    {
                        var previousEnd = ends[previousEndIndex].Value;

                        if (previousEnd.EndTime >= time)
                        {
                            EventReverted?.Invoke(previousEnd);
                            previousEndIndex--;
                        }
                        else break;
                    }
                }
                else if (previousEndIndex == -1)
                {
                    var previousStart = this[previousStartIndex];

                    if (previousStart.StartTime > time)
                    {
                        EventRewound?.Invoke(previousStart);
                        previousStartIndex--;
                    }
                    else break;
                }
                else
                {
                    var previousStart = this[previousStartIndex];
                    var previousEnd = ends[previousEndIndex].Value;

                    // first un-start then un-end
                    if (previousStart.StartTime >= previousEnd.EndTime)
                    {
                        if (previousStart.StartTime > time)
                        {
                            EventRewound?.Invoke(previousStart);
                            previousStartIndex--;
                        }
                        else if (previousEnd.EndTime >= time)
                        {
                            EventReverted?.Invoke(previousEnd);
                            previousEndIndex--;
                        }
                        else break;
                    }
                    else
                    {
                        if (previousEnd.EndTime >= time)
                        {
                            EventReverted?.Invoke(previousEnd);
                            previousEndIndex--;
                        }
                        else break;
                    }
                }
            }

            currentTime = time;
        }

        /// <summary>
        /// Seeks backward to right before the given time. This assumes that the last operation seeked to the last event at that time instant.
        /// </summary>
        private void seekBackwardToBefore(double time)
        {
            int previousStartIndex = FirstBeforeOrAt(currentTime);
            int previousEndIndex = ends.FirstBefore(currentTime);

            while (true)
            {
                if (previousStartIndex == -1)
                {
                    if (previousEndIndex == -1)
                    {
                        break;
                    }
                    else
                    {
                        var previousEnd = ends[previousEndIndex].Value;

                        if (previousEnd.EndTime > time)
                        {
                            EventReverted?.Invoke(previousEnd);
                            previousEndIndex--;
                        }
                        else break;
                    }
                }
                else if (previousEndIndex == -1)
                {
                    var previousStart = this[previousStartIndex];

                    if (previousStart.StartTime >= time)
                    {
                        EventRewound?.Invoke(previousStart);
                        previousStartIndex--;
                    }
                    else break;
                }
                else
                {
                    var previousStart = this[previousStartIndex];
                    var previousEnd = ends[previousEndIndex].Value;

                    // first un-start then un-end
                    if (previousStart.StartTime >= previousEnd.EndTime)
                    {
                        if (previousStart.StartTime >= time)
                        {
                            EventRewound?.Invoke(previousStart);
                            previousStartIndex--;
                        }
                        else break;
                    }
                    else
                    {
                        if (previousEnd.EndTime > time)
                        {
                            EventReverted?.Invoke(previousEnd);
                            previousEndIndex--;
                        }
                        else break;
                    }
                }
            }

            currentTime = time;
        }

        /// <summary>
        /// Seeks forward to right after the given time. This assumes that the last operation seeked to the last event before that time instant.
        /// </summary>
        private void seekForwardToAfter(double time)
        {
            int nextStartIndex = FirstAfterOrAt(currentTime);
            int nextEndIndex = ends.FirstAfterOrAt(currentTime);

            while (true)
            {
                if (nextEndIndex >= ends.Count) nextEndIndex = -1;
                if (nextStartIndex >= Count) nextStartIndex = -1;

                if (nextStartIndex == -1)
                {
                    if (nextEndIndex == -1)
                    {
                        break;
                    }
                    else
                    {
                        var nextEnd = ends[nextEndIndex].Value;

                        if (nextEnd.EndTime < time)
                        {
                            EventEnded?.Invoke(nextEnd);
                            nextEndIndex++;
                        }
                        else break;
                    }
                }
                else if (nextEndIndex == -1)
                {
                    var nextStart = this[nextStartIndex];

                    if (nextStart.StartTime <= time)
                    {
                        EventStarted?.Invoke(nextStart);
                        nextStartIndex++;
                    }
                    else break;
                }
                else
                {
                    var nextStart = this[nextStartIndex];
                    var nextEnd = ends[nextEndIndex].Value;

                    // first end, then start
                    if (nextEnd.EndTime <= nextStart.StartTime)
                    {
                        if (nextEnd.EndTime < time)
                        {
                            EventEnded?.Invoke(nextEnd);
                            nextEndIndex++;
                        }
                        else if (nextStart.StartTime <= time)
                        {
                            EventStarted?.Invoke(nextStart);
                            nextStartIndex++;
                        }
                        else break;
                    }
                    else
                    {
                        if (nextStart.StartTime <= time)
                        {
                            EventStarted?.Invoke(nextStart);
                            nextStartIndex++;
                        }
                        else break;
                    }
                }
            }

            currentTime = time;
        }

        /// <summary>
        /// All entries active at a given time peroid, not ordered. This operation is expensive O(n).
        /// </summary>
        public IEnumerable<TimelineSeeker<T>.Entry> EntriesBetween(double start, double end)
        {
            var nextStartIndex = FirstAfterOrAt(start);
            var nextEndIndex = ends.FirstAfterOrAt(start);

            // all that end within the peroid
            if (nextEndIndex != -1)
            {
                while (nextEndIndex < ends.Count)
                {
                    var entry = ends[nextEndIndex++].Value;
                    if (entry.EndTime <= end)
                    {
                        // excpet the ones that start within it
                        if (entry.StartTime < start)
                        {
                            yield return entry;
                        }
                    }
                    else
                        break;
                }
            }

            // all that start withing the peroid
            if (nextStartIndex != -1)
            {
                while (nextStartIndex < Count)
                {
                    var entry = this[nextStartIndex++];
                    if (entry.StartTime <= end)
                        yield return entry;
                    else
                        break;
                }
            }

            nextEndIndex = ends.FirstAfter(end);
            // all that span over the peroid
            if (nextEndIndex != -1)
            {
                while (nextEndIndex < ends.Count)
                {
                    var entry = ends[nextEndIndex++].Value;
                    if (entry.StartTime < start)
                        yield return entry;
                }
            }
        }

        public delegate void EventEncounteredHandler(Entry entry);
        /// <summary>
        /// While seeking in positive direction, an event was rised.
        /// </summary>
        public event EventEncounteredHandler? EventStarted;
        /// <summary>
        /// While seeking in positive direction, an event was dropped.
        /// </summary>
        public event EventEncounteredHandler? EventEnded;
        /// <summary>
        /// While seeking in a negative direction, reverted an event's drop.
        /// </summary>
        public event EventEncounteredHandler? EventReverted;
        /// <summary>
        /// While seeking in a negative direction, rewound an event's rise.
        /// </summary>
        public event EventEncounteredHandler? EventRewound;

        public class Entry : TimelineEntry<T>
        {
            public readonly double Duration;
            public double EndTime => StartTime + Duration;

            public Entry(double startTime, double duration, T value) : base(startTime, value)
            {
                Duration = duration;
            }
        }
    }

    public enum TimelineModifiedBehaviour
    {
        /// <summary>
        /// Does nothing, only rises the <see cref="Timeline{T, E}.EntryAdded"/> or <see cref="Timeline{T, E}.EntryRemoved"/> events.
        /// </summary>
        None,
        /// <summary>
        /// If the entry starts before or at <see cref="TimelineSeeker{T}.CurrentTime"/>, reverts to the instant before the entry's start time,
        /// before ading/removing it and then seeks back to <see cref="TimelineSeeker{T}.CurrentTime"/>.
        /// </summary>
        Replay
    }
}
