using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAfrica.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string ProfilePic { get; set; }
        public string InternationalPrefix { get; set; }
        public string Address { get; set; }
        public float CoordinateX { get; set; }
        public float CoordinateY { get; set; }
        public string AddressLine { get; set; }
        public string AdditionalDetails { get; set; }
        public string Source { get; set; }
        public string PushTokenId { get; set; }
        public string OrganizationName { get; set; }
        public string EmergencyContactNo { get; set; }
        public string IncidentTypeId { get; set; }
       public bool IsVolunteer { get; set; }

        public double Distance { get; set; }
    }
}
