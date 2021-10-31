using System;

namespace osu.Game.Rulesets.Hitokori.Edit.Connectors {
	/// <summary>
	/// Indiactes that this property can be inspected in the beatmap editor.
	/// </summary>
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class InspectableAttribute : Attribute {
		public bool IsReadonly;
		public string? Label;
		public string? Section;

		public string? FormatMethod;
		public string? ParseMethod;

		public const string SectionTiming = "Timing";
		public const string SectionProperties = "Properties";
	}
}
