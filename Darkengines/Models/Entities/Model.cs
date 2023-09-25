using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Models.Entities {
    public class Model {
        public Model() {
            Entities = new Collection<Entity>();
        }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ICollection<Entity> Entities { get; }
    }
}
