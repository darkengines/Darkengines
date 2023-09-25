namespace Darkengines.Models.Entities {
    public class ForeignKeyProperty {
        public string? ModelName { get; set; }
        public string? ForeignKeyName { get; set; }
        public virtual ForeignKey? ForeignKey { get; set; }
        public string? DependentEntityName { get; set; }
        public string? DependentPropertyName { get; set; }
        public virtual Entity? DependentEntity { get; set; }
        public virtual Property? DependentProperty { get; set; }

        public string? PrincipalEntityName { get; set; }
        public string? PrincipalPropertyName { get; set; }
        public virtual Entity? PrincipalEntity { get; set; }
        public virtual Property? PrincipalProperty { get; set; }
    }
}