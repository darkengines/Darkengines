using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Models.Entities {
    public abstract class Navigation : Member {
        public string? ForeignKeyName { get; set; }
        public ForeignKey? ForeignKey { get; set; }
    }
}
