using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Game.Rulesets.Hitokori.Utils;

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot {
	public class Bot : Container {
		private AutoClickType clickType = AutoClickType.Up;
		private double clickTime;

		BotButton Left;
		BotButton Right;
		BotButton Active;
		BotButton Inactive => ( Active == Left ) ? Right : Left;

		Sprite Texture;

		Texture LeftTexture;
		Texture RightTexture;
		Texture NeutralTexture;
		Texture HoldTexture;

		bool IsHolding;

		public Bot () {
			Active = Left = new BotButton();
			Right = new BotButton();

			Height = 130;
			Width = 130;

			AddRangeInternal( new Drawable[] {
				Texture = new Sprite {
					RelativeSizeAxes = Axes.Both,
					FillMode = FillMode.Fill
				}.Center()
			} );
		}

		public void Press () {
			if ( clickType != AutoClickType.Up ) {
				Swap();
			}

			clickType = AutoClickType.Press;
			clickTime = Clock.CurrentTime;
			Active.Hold();
		}

		public void Hold () {
			if ( clickType != AutoClickType.Up ) {
				Swap();
			}

			IsHolding = true;
			clickType = AutoClickType.Down;
			clickTime = Clock.CurrentTime;
		}

		public void Release () {
			if ( clickType == AutoClickType.Down ) {
				Swap();
				clickType = AutoClickType.Up;
			}
		}

		void Swap () {
			Active.Release();
			Active = Inactive;
			IsHolding = false;
		}


		private bool CanGhost = false;
		private double CanGhostFrom;
		public void AllowGhosting () {
			CanGhost = true;
			CanGhostFrom = Clock.CurrentTime;
		}

		public void ForbidGhosting () {
			CanGhost = false;
		}

		protected override void Update () {
			if ( CanGhost && ( Clock.CurrentTime - CanGhostFrom ) > 2000 && clickType == AutoClickType.Up ) {
				clickType = AutoClickType.Press;
				clickTime = Clock.CurrentTime - 25;

				Active.Hold();
			}

			if ( clickType == AutoClickType.Press && clickTime + 100 < Clock.CurrentTime ) {
				Swap();
				clickType = AutoClickType.Up;
			}

			if ( IsHolding ) {
				Texture.Texture = HoldTexture;
			} else if ( Left.IsDown ) {
				Texture.Texture = LeftTexture;
			} else if ( Right.IsDown ) {
				Texture.Texture = RightTexture;
			} else {
				Texture.Texture = NeutralTexture;
			}
		}

		[BackgroundDependencyLoader]
		private void load () {
			RightTexture = GetTexture( "hitokori-BotRight" );
			LeftTexture = GetTexture( "hitokori-BotLeft" );
			NeutralTexture = GetTexture( "hitokori-BotNeutral" );
			HoldTexture = GetTexture( "hitokori-BotHold" );

			Texture.FillAspectRatio = HoldTexture.Width / (float)HoldTexture.Height;
		}

		Texture GetTexture ( string name ) { // TODO please for the love of thfgjygdsf make this better ( and skinable! )
			return new TextureStore(
				new TextureLoaderStore(
					new NamespacedResourceStore<byte[]>(
						new DllResourceStore( GetType().Assembly ),
						@"Resources"
					)
				),
				false
			).Get( name );
		}
	}
}
