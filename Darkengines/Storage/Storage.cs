using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Storage {
	public class Storage
    {
        protected StorageOptions Options { get; }
        public Storage(IOptions<StorageOptions> options)
        {
            Options = options.Value;
        }
    }
}
