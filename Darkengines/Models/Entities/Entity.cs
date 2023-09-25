using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Models.Entities {
    public class Entity {
        public Entity() {
            Members = new Collection<Member>();
        }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? ModelName { get; set; }
        public string? Description { get; set; }
        public Model? Model { get; set; }
        public ICollection<Member> Members { get; }
    }
}
