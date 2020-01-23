using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAfrica.DataModel
{
    public class IncidentRequest
    {
        public string UserAccountId { get; set; }
        public string EmergencyAccountId { get; set; }
        public string IncidentTypeId { get; set; }

        public string IncidentPic { get; set; }
        public string Address { get; set; }
        public string AddressLine { get; set; }
        public float CoordinateX { get; set; }
        public float CoordinateY { get; set; }
        public string CustomerComments { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool Deleted { get; set; }
    }
}
