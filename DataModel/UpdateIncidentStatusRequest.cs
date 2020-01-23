using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAfrica.DataModel
{
    public class UpdateIncidentStatusRequest
    {
        public string Id { get; set; }
        public string EmergencyAccountId { get; set; }
        public string Status { get; set; }
    }
}
