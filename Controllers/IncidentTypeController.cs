using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureAfrica.DataModel;
using SecureAfrica.Models;

namespace SecureAfrica.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentTypeController : ControllerBase
    {
        public AppDbContex appDbContex { get; }

        public IncidentTypeController(AppDbContex _appDbContex)
        {
            this.appDbContex = _appDbContex;
        }
        [HttpPost]
        [Route("AddIncidentType")]
        public ActionResult AddIncidentType(IncidentTypeRequest incidentTypeRequest)
        {
            var result = appDbContex.IncidentTypes.Where(a => a.Name == incidentTypeRequest.incidenttypename).FirstOrDefault();

            if (result == null)
            {
                string id = Guid.NewGuid().ToString();
                IncidentType incidentType = new IncidentType()
                {
                    Id = id,
                    Name = incidentTypeRequest.incidenttypename
                };

                appDbContex.Add(incidentType);
                appDbContex.SaveChangesAsync();
                return Ok(new { Message = "Incident Type Added Successfully..!" });
               
            }

            return BadRequest(new { Message = "Incident Type Already Exist..!" });

        }
        [HttpPost]
        [Route("updateIncidentType")]
        public ActionResult updateIncidentType(IncidentTypeRequest incidentTypeRequest)
        {

            var incidentname = appDbContex.IncidentTypes.Where(a => a.Name == incidentTypeRequest.incidenttypename && a.Id != incidentTypeRequest.id).SingleOrDefault();
            if (incidentname == null)
            {
                var incidenttype = appDbContex.IncidentTypes.Where(a => a.Id == incidentTypeRequest.id).SingleOrDefault();
                if (incidenttype != null)
                {
                    incidenttype.Name = incidentTypeRequest.incidenttypename;
                   // country.CountryCode = countryRequest.CountryCode;
                    appDbContex.Update(incidenttype);
                    appDbContex.SaveChangesAsync();
                    //return new resultmv
                    //{ Status = "Success", Message = "Record updated successfully!" };

                    return Ok(new { Message = "Record updated successfully!" });
                }
                return BadRequest(new { Message = "Record not exixst!" });

            }

            return BadRequest(new { Message = "Incident Name alredy exixst!" });
        }

        [HttpGet]
        [Route("GetAllIncidentType")]
        public ActionResult GetAllIncidentType()
        {
           List<IncidentType> incidentType = appDbContex.IncidentTypes.ToList();

            return Ok(incidentType);
        }
        [HttpPost]
        [Route("FindIncidentTypeById")]
        public ActionResult FindIncidentTypeById(string id)
        {
            IncidentType incidentType = appDbContex.IncidentTypes.Where(a => a.Id == id).FirstOrDefault();

            return Ok(incidentType);
        }
    }
}