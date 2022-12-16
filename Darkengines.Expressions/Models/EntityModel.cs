using System.Collections.Generic;

namespace Darkengines.Expressions.Models {
	public class EntityModel {
		public string Name { get; set; }
		public string[] Interfaces { get; set; }
		public string DisplayTypeName { get; set; }
		public string DisplayName { get; set; }
		public EntityModel Parent { get; set; }
		public ReferenceModel[] References { get; set; }
		public CollectionModel[] Collections { get; set; }
		public PropertyModel[] Properties { get; set; }
		public PropertyModel[] PrimaryKey { get; set; }
		public PropertyModel[] SummaryProperties { get; set; }
		public HashSet<EntityModel> Dependents { get; set; } = new HashSet<EntityModel>();
		public HashSet<EntityModel> CollectionDependents { get; set; } = new HashSet<EntityModel>();
		public string Module { get; set; }
		public string[] Namespace { get; set; }
	}
}
