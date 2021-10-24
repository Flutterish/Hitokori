using osu.Game.Rulesets.Hitokori.Collections;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Hitokori.Orbitals.Events {
	public class VisualEventSeeker : TimelineSeeker<VisualEvent> {
		private long nextOrder;

		public VisualEventSeeker () : base( Comparer<VisualEvent>.Create( ( a, b ) => Math.Sign( a.Order - b.Order ) ) ) {
			ModifiedBehaviour = TimelineModifiedBehaviour.Replay;

			EventStarted += e => {
				TimeSeeked?.Invoke( e.StartTime );
				apply( e.StartTime );
				startVisualEvent( e.Value );
			};
			EventEnded += e => {
				TimeSeeked?.Invoke( e.EndTime );
				apply( e.EndTime );
				endVisualEvent( e.Value );
			};
			EventReverted += e => {
				TimeSeeked?.Invoke( e.EndTime );
				revertVisualEvent( e.Value );
				apply( e.EndTime );
			};
			EventRewound += e => {
				TimeSeeked?.Invoke( e.StartTime );
				apply( e.StartTime );
				rewindVisualEvent( e.Value );
			};
		}

		public int Add ( VisualEvent e ) {
			return Add( e.StartTime, e.Duration, e );
		}

		public override int Add ( Entry entry ) {
			entry.Value.Order = nextOrder++;
			return base.Add( entry );
		}

		public event Action<double>? TimeSeeked;

		Dictionary<string, VisualEvent> visualEventCategoriesTrackers = new();
		List<(VisualEvent obscurer, VisualEvent obscuree)> obscuredVisualEvents = new();
		List<VisualEvent> activeVisualEvents = new();

		private void startVisualEvent ( VisualEvent e ) {
			foreach ( var category in e.Categories ) {
				if ( visualEventCategoriesTrackers.Remove( category, out var @event ) ) {
					if ( activeVisualEvents.Remove( @event ) ) {
						@event.InterruptedTime = e.StartTime;
						@event.Obscurer = e;
						obscuredVisualEvents.Add( (e, @event) );
					}
				}

				visualEventCategoriesTrackers.Add( category, e );
			}

			activeVisualEvents.Add( e );
		}

		private void endVisualEvent ( VisualEvent e ) {
			if ( activeVisualEvents.Remove( e ) ) {
				foreach ( var category in e.Categories ) {
					visualEventCategoriesTrackers.Remove( category );
				}
			}

			for ( int i = 0; i < obscuredVisualEvents.Count; i++ ) {
				if ( obscuredVisualEvents[ i ].obscuree == e ) {
					obscuredVisualEvents.RemoveAt( i-- );
				}
			}
		}

		private void revertVisualEvent ( VisualEvent e ) {
			if ( e.InterruptedTime < e.EndTime ) {
				obscuredVisualEvents.Add( (e.Obscurer!, e) );
			}
			else {
				foreach ( var category in e.Categories ) {
					if ( visualEventCategoriesTrackers.Remove( category, out var @event ) ) {
						activeVisualEvents.Remove( @event );
					}

					visualEventCategoriesTrackers.Add( category, e );
				}

				activeVisualEvents.Add( e );
			}
		}

		private void rewindVisualEvent ( VisualEvent e ) {
			e.Revert();

			if ( activeVisualEvents.Remove( e ) ) {
				foreach ( var category in e.Categories ) {
					visualEventCategoriesTrackers.Remove( category );
				}

				for ( int i = obscuredVisualEvents.Count - 1; i >= 0; i-- ) {
					var (obscurer, obscuree) = obscuredVisualEvents[ i ];

					if ( obscurer == e ) {
						obscuredVisualEvents.RemoveAt( i );

						obscuree.Obscurer = null;
						obscuree.InterruptedTime = double.PositiveInfinity;

						foreach ( var cat in obscuree.Categories ) {
							visualEventCategoriesTrackers.Add( cat, obscuree );
						}

						activeVisualEvents.Add( obscuree );
					}
				}
			}
		}

		public void Apply () {
			apply( CurrentTime );
		}

		private void apply ( double time ) {
			foreach ( var i in activeVisualEvents ) {
				i.ApplyAt( time );
			}
		}
	}
}
