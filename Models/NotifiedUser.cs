using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAfrica
{
    public class NotifiedUser
    {
        public string Id { get; set; }
        public string IncidentId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
