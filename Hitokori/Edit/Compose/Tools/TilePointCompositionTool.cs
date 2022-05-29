using osu.Framework.Allocation;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Hitokori.Edit.Compose.Connectors;
using osu.Game.Rulesets.Hitokori.Edit.Compose.SelectionOverlays;
using osu.Game.Rulesets.Hitokori.Objects;
using osu.Game.Rulesets.Hitokori.Objects.TilePoints;
using osu.Game.Rulesets.Hitokori.Orbitals;
using osu.Game.Rulesets.Hitokori.UI;
using osu.Game.Rulesets.Hitokori.UI.Visuals;
using osuTK.Graphics;
using osuTK.Input;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Rulesets.Hitokori.Edit.Compose.Tools {
	public class TilePointCompositionTool : HitObjectCompositionTool {
		public TilePointCompositionTool ( string name ) : base( name ) { }

		public override Drawable CreateIcon ()
			=> new SpriteIcon { Icon = FontAwesome.Solid.Link };

		public override PlacementBlueprint CreatePlacementBlueprint ()
			=> new TilePointPlacementBlueprint( new SwapTilePoint() );
	}

	public class TilePointPlacementBlueprint : PlacementBlueprint, IRequireHighFrequencyMousePosition {
		[Resolved, MaybeNull, NotNull]
		public HitokoriPlayfield Playfield { get; private set; }
		[Resolved, MaybeNull, NotNull]
		public HitokoriHitObjectComposer Composer { get; private set; }

		new public TilePoint HitObject => (TilePoint)base.HitObject;
		private TilePointConnector? connector;
		private ConnectorBlueprint? connectorBlueprint;
		private PathVisualizer pathVisualizer;

		Container<Drawable> glow;
		TilePointVisualPiece piece;

		public TilePointPlacementBlueprint ( TilePoint hitObject ) : base( hitObject ) {
			AddInternal( glow = (piece = new TilePointVisualPiece {
				
			}).WithEffect(new GlowEffect { PadExtent = true } ) );

			glow.Anchor = Anchor.TopLeft;

			AddInternal( pathVisualizer = new PathVisualizer { Colour = Color4.Yellow, Depth = -1000 } );
			pathVisualizer.Hide();

			piece.InAnimationProgress.Value = 1;
			piece.OutAnimationProgress.Value = 1;
		}

		new PlacementState PlacementActive = PlacementState.Waiting; // I dont know why, but the base is always "Finished" when disposing
		protected override void LoadComplete () {
			base.LoadComplete();

			PlacementActive = PlacementState.Active;
			BeginPlacement( commitStart: true );
		}

		public override bool ReceivePositionalInputAt ( Vector2 screenSpacePos )
			=> true;

		protected override void Update () {
			base.Update();

			glow.Scale = Playfield.Scale;

			var closest = makeNewChain ? null : ClosestTile();

			var pos = makeNewChain
				? targetPosition
				: snapPosition( closest, targetPosition );

			if ( closest is null ) {
				if ( connector is not null ) {
					removeConnector();

					Composer.UpdateVisuals();
				}

				HitObject.StartTime = Composer.EditorBeatmap.SnapTime( Playfield.Clock.CurrentTime, null );

				if ( !HitObject.ConstrainableOrbitalState.IsConstrained ) {
					HitObject.ConstrainOrbitalState = new OrbitalState( new[] {
						(Math.Tau / 2 * 0).AngleToVector(0.5f),
						(Math.Tau / 2 * 1).AngleToVector(0.5f)
					} ).RotatedBy( Math.PI );
				}
				HitObject.ConstrainPosition = pos;
				HitObject.ConstrainOrbitalState = HitObject.OrbitalState.PivotNth( 0, pos );

				glow.Position = ToLocalSpace( Playfield.ScreenSpacePositionOf( (Vector2)pos ) );
				piece.AroundPosition.Value = (Vector2d)piece.Position;
				piece.FromPosition.Value = null;
			}
			else {
				if ( closest.Next is null ) {
					HitObject.StartTime = closest.StartTime + Composer.Beatmap.BeatLengthAt( closest.StartTime ) / 2;

					if ( connector is null ) {
						HitObject.ChainID = closest.ChainID;
						Composer.Link( closest, HitObject, connector = createConnector() );
					}
					else if ( connector.From != closest ) {
						removeConnector();

						HitObject.ChainID = closest.ChainID;
						Composer.Link( closest, HitObject, connector = createConnector() );
					}

					piece.FromPosition.Value = closest.Position;
					piece.ToPosition.Value = null;

					Composer.UpdateVisuals();
				}
				// TODO previous
			}

			if ( connector is not null ) {
				glow.Position = ToLocalSpace( Playfield.ScreenSpacePositionOf( (Vector2)HitObject.Position ) );
				piece.AroundPosition.Value = HitObject.Position;

				pathVisualizer.Scale = new Vector2( (float)Composer.Playfield.CameraScale.Value );
				pathVisualizer.Position = ToLocalSpace( Composer.Playfield.ScreenSpacePositionOf( (Vector2)connector.From.Position ) );
				pathVisualizer.TilePosition = (Vector2)connector.From.Position;
			}
		}

		public override void UpdateTimeAndPosition ( SnapResult result ) {
			// no
		}

		protected override bool OnClick ( ClickEvent e ) {
			if ( e.Button == MouseButton.Left ) {
				PlacementActive = PlacementState.Finished;
				EndPlacement( true );

				return true;
			}

			return false;
		}

		private TilePointConnector createConnector () {
			if ( connectorBlueprint is not null ) {
				RemoveInternal( connectorBlueprint );
				connectorBlueprint.Dispose();
				connectorBlueprint = null;
			}

			connector = new TilePointRotationConnector();
			connectorBlueprint = connector.CreateEditorBlueprint();
			AddInternal( connectorBlueprint );
			pathVisualizer.VisualizedConnector.Value = connector;
			pathVisualizer.Show();

			connectorBlueprint.Modified += () => {
				Composer.UpdateVisuals();
			};

			return connector;
		}

		private void removeConnector () {
			pathVisualizer.VisualizedConnector.Value = null;
			pathVisualizer.Hide();

			if ( connectorBlueprint is not null ) {
				RemoveInternal( connectorBlueprint );
				connectorBlueprint.Dispose();
				connectorBlueprint = null;
			}

			if ( connector!.To == HitObject ) {
				connector.From = null;
				connector.To = null;
				connector = null;

				Composer.UpdateOrbitals( HitObject.ChainID );
			}

			Composer.UpdateVisuals();
		}

		private LocalisableString tooltip = "";
		private LocalisableString Tooltip {
			get => tooltip;
			set => Composer.Toast.ShowMessage( tooltip = value );
		}
		protected override void Dispose ( bool isDisposing ) {
			base.Dispose( isDisposing );

			Composer.Toast.HideMessage( Tooltip );

			connectorBlueprint = null;
			if ( connector is not null && PlacementActive != PlacementState.Finished ) {
				Composer.Schedule( () => {
					removeConnector();
				} );
			}
		}

		public TilePoint? ClosestTile () {
			TilePoint? tp = null;
			foreach ( var i in Playfield.AliveTiles.Where( x => x != HitObject && ( x.Next is null || x.ToNext == connector ) ) ) {
				if ( tp is null ) {
					tp = i;
				}
				else if ( ( targetPosition - i.Position ).Length < ( targetPosition - tp.Position ).Length ) {
					tp = i;
				}
			}

			return tp;
		}

		private Vector2d snapPosition ( TilePoint? around, Vector2d target ) {
			if ( around is null )
				return target;

			if ( target == around.Position )
				return target + new Vector2d( 1, 0 );
			else
				return around.Position + ( target - around.Position ).Normalized();
		}

		Vector2d targetPosition;
		private bool makeNewChain = false;
		protected override bool OnMouseMove ( MouseMoveEvent e ) {
			piece.Alpha = 1;
			targetPosition = (Vector2d)Playfield.NormalizedPositionAtScreenSpace( e.ScreenSpaceMousePosition );

			if ( e.AltPressed ) {
				makeNewChain = true;
				Tooltip = "[Alt] new chain";
			}
			else {
				makeNewChain = false;
				if ( string.IsNullOrWhiteSpace( connectorBlueprint?.Tooltip ) ) {
					Tooltip = "Alt to create new chain";
				}
				else {
					Tooltip = $"Alt to create new chain | {connectorBlueprint.Tooltip}";
				}
			}

			return base.OnMouseMove( e );
		}
	}
}
