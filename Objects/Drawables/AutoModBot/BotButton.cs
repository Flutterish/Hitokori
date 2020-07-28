

namespace osu.Game.Rulesets.Hitokori.Objects.Drawables.AutoModBot {
	public class BotButton {
		public BotButton () {
			Release();
		}

		public bool IsDown { get; private set; }

		public void Hold () {
			IsDown = true;
		}

		public void Release () {
			IsDown = false;
		}
	}
}
