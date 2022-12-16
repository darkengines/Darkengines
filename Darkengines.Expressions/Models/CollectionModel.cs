namespace Darkengines.Expressions.Models {
	public class CollectionModel {
		public string Name { get; set; }
		public string DisplayTypeName { get; set; }
		public EntityModel? Type { get; set; }
		public PropertyModel[] ForeignKey { get; set; }
		public PropertyModel[] TargetForeignKey { get; set; }
		public bool IsDependentToPrincipal { get; set; }
	}
}
