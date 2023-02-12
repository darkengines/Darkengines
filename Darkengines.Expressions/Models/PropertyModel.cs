﻿namespace Darkengines.Expressions.Models {
	public class PropertyModel {
		public string Name { get; set; }
		public bool IsNullable { get; set; }
		public string TypeName { get; set; }
		public string DisplayTypeName { get; set; }
		public int? MaxLength { get; set; }
		public string Format { get; set; }
		public bool IsAutoGenerated { get; set; }
	}
}