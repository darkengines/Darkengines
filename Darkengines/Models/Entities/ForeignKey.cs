using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Models.Entities {
    public class ForeignKey {
        public ForeignKey() {
            Properties = new Collection<ForeignKeyProperty>();
            Navigations = new Collection<Navigation>();
        }
        public string? ModelName { get; }
        public Model? Model { get; set; }
        public string? Name { get; set; }
        public ICollection<ForeignKeyProperty> Properties { get; }
        public ICollection<Navigation> Navigations { get; }
    }
}
