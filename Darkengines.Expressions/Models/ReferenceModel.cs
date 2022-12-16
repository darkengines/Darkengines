namespace Darkengines.Expressions.Models {
	public class ReferenceModel {
		public string Name { get; set; }
		public bool IsNullable { get; set; }
		public EntityModel Type { get; set; }
		public PropertyModel[] ForeignKey { get; set; }
		public PropertyModel[] TargetForeignKey { get; set; }
		public bool IsDependentToPrincipal { get; set; }
	}
}
