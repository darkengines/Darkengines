using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Models.Entities {
    public abstract class Member {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? ModelName { get; set; }
        public string? EntityName { get; set; }
        public Entity? Entity { get; set; }
        public string? MemberType { get; set; }
    }
}
