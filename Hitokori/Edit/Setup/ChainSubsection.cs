using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Rulesets.Hitokori.Beatmaps;

namespace osu.Game.Rulesets.Hitokori.Edit.Setup {
	public class ChainSubsection : SetupSubsection {
		Chain chain;
		public ChainSubsection ( Chain chain ) {
			this.chain = chain;
		}

		protected override void LoadComplete () {
			base.LoadComplete();

			Add( new LabelledTextBox { FixedLabelWidth = LABEL_WIDTH, Label = "Name", Current = chain.NameBindable } );
			Add( new LabelledNumberBox { FixedLabelWidth = LABEL_WIDTH, Label = "ID", Current = new Bindable<string>( chain.ID.ToString() ), ReadOnly = true } );
		}

		public override LocalisableString Title => $"Chain {chain.ID}";
	}
}
