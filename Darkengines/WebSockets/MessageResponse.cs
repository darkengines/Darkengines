using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.WebSockets {
    public class MessageResponse {
        public long Id { get; set; }
        public bool Success { get; set; }
        public Response Response { get; set; }
    }
}
