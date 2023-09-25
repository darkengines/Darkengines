using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.WebSockets {
    public class MessageRequest {
        public long Id { get; set; }
        public Request? Request { get; set; }
    }
}
