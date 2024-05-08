using Darkengines.Data;
using Darkengines.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Storage.Entities {
	public class File : IMonitored {
		public int Id { get; set; }
		public required string ContentType { get; set; }
		public required long Length { get; set; }
		public required Uri Uri { get; set; }
		public int? CreatedById { get; set; }
		public User? CreatedBy { get; set; }
		public DateTimeOffset? CreatedOn { get; set; }
		public int? ModifiedById { get; set; }
		public User? ModifiedBy { get; set; }
		public DateTimeOffset? ModifiedOn { get; set; }
	}
}
