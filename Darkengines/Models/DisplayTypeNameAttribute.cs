using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Models {
    public class DisplayTypeNameAttribute : Attribute {
        public string DisplayTypeName { get; }
        public DisplayTypeNameAttribute(string displayTypeName) {
            DisplayTypeName = displayTypeName;
        }
    }
}
