using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Hitokori.Objects.Base;
using osu.Game.Rulesets.Hitokori.Objects.Drawables.Tiles;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using System.ComponentModel;
using Container = osu.Framework.Graphics.Containers.Container;
using Vector2 = osuTK.Vector2;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables {
	public class DrawableHitokoriJudgement : DrawableJudgement, IHasTilePosition {
		private HitResult Type;
		private double Offset;

		public DrawableHitokoriJudgement ( JudgementResult result, DrawableTilePoint judgedObject ) : base( result, judgedObject ) {
			TilePosition = judgedObject.TilePoint.TilePosition;
			Type = result.Type;
			Offset = result.TimeOffset;
		}

		public Vector2 TilePosition { get; set; }
		public OsuSpriteText Text;

		[Resolved]
		private OsuColour colours { get; set; }

		[BackgroundDependencyLoader]
		private void load () {
			InternalChild = new Container {
				Anchor = Anchor.Centre,
				Origin = Anchor.Centre,
				RelativeSizeAxes = Axes.Both,
				Child = Text = new OsuSpriteText {
					Anchor = Anchor.Centre,
					Origin = Anchor.Centre,
					Text = Type.GetDescription().ToUpperInvariant(),
					Font = OsuFont.Numeric.With( size: 20 ),
					Colour = colours.ForHitResult( Type ),
					Scale = new Vector2( 0.5f ),
					Position = new Vector2( 0, -45 )
				}
			};

			var result = GetADOFAIResult();
			Text.Text = result.GetDescription().ToUpperInvariant();
		}

		protected override void ApplyHitAnimations () {
			Text.FadeInFromZero( 80 ).TransformSpacingTo( new Vector2( 10 ), 500, Easing.Out ).Then().Delay( 300 ).Then().FadeOut( 300, Easing.Out );
			LifetimeEnd = LifetimeStart + 2000;
			base.ApplyHitAnimations();
		}

		protected override void ApplyMissAnimations () {
			Text.FadeInFromZero( 80 ).TransformSpacingTo( new Vector2( 10 ), 500, Easing.Out ).Then().Delay( 300 ).Then().FadeOut( 300, Easing.Out );
			LifetimeEnd = LifetimeStart + 2000;
			base.ApplyMissAnimations();
		}

		public ADOFAIResult GetADOFAIResult () {
			if ( Type == HitResult.None ) {
				return ADOFAIResult.None;
			}

			if ( Type == HitResult.Miss ) {
				return ADOFAIResult.Miss;
			}

			if ( Type != HitResult.Perfect ) {
				if ( Offset > 0 ) {
					return ADOFAIResult.Late;
				}
				if ( Offset < 0 ) {
					return ADOFAIResult.Early;
				}
			}

			return ADOFAIResult.Perfect;
		}
	}

	public enum ADOFAIResult {
		[Description( "" )]
		None,

		[Description( "Miss" )]
		Miss,

		[Description( "Early" )]
		Early,

		[Description( "Late" )]
		Late,

		[Description( "Perfect" )]
		Perfect
	}
}
