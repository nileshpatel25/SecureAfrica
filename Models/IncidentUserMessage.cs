using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAfrica.Models
{
    public class IncidentUserMessage
    {
        public string Id { get; set; }
        public string IncidentId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string StatusMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
