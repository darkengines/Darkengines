using Darkengines.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Users.Entities {
    public class UserProfile : IMonitored {
        public int Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? ImageUrl { get; set; }
        public Gender? Gender { get; set; }
        public User User { get; set; }
        public int? CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public int? ModifiedById { get; set; }
        public User? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}
