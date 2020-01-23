using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureAfrica.Models;
using SecureAfrica.DataModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace SecureAfrica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        public AppDbContex appDbContex { get; }
        public CountryController(AppDbContex _appDbContex)
        {
            this.appDbContex = _appDbContex;
        }

      

        [HttpPost("addnewCountry")]
        public ActionResult addnewCountry(CountryRequest countryRequest)
        {
            var countryname = appDbContex.Countries.Where(a => a.CountryName == countryRequest.CountryName).FirstOrDefault();
            if (countryname == null)
            {
                string id = Guid.NewGuid().ToString();
                Country country = new Country
                {
                    Id = id,
                    CountryName = countryRequest.CountryName,
                    CountryCode = countryRequest.CountryCode
                };
                appDbContex.Countries.Add(country);
                appDbContex.SaveChangesAsync();
                return Ok(new { Message = "Country Name add successfully!" });

                // return Ok("Country Name add successfully!");

            }
            return BadRequest(new { Message = "Country Name Already Exists!" });
        }

        [HttpPost("updateCountry")]
        public async Task<ActionResult> updateCountry(CountryRequest countryRequest)
        {

            var countryname = appDbContex.Countries.Where(a => a.CountryName == countryRequest.CountryName && a.Id != countryRequest.id).SingleOrDefault();
            if (countryname == null)
            {
                var country = appDbContex.Countries.Where(a => a.Id == countryRequest.id).SingleOrDefault();
                if (country != null)
                {
                    country.CountryName = countryRequest.CountryName;
                    country.CountryCode = countryRequest.CountryCode;
                    appDbContex.Update(country);
                    await appDbContex.SaveChangesAsync();
                    //return new resultmv
                    //{ Status = "Success", Message = "Record updated successfully!" };
                  
                   return Ok( new { Message = "Record updated successfully!" });
                }
                return BadRequest(new { Message = "Record not exixst!"});

            }

            return BadRequest(new { Message = "Country Name alredy exixst!" });
        }


        [HttpGet("getAllCountry")]
        public ActionResult getallCountry()
        {
            List<Country> countries = appDbContex.Countries.ToList();
            return Ok(countries);
        }

        [HttpPost]
        [Route("deletebyId")]
        public ActionResult deletbyId(string id)
        {
            var countryname = appDbContex.Countries.Where(a => a.Id == id).FirstOrDefault();
            if (countryname != null)
            {
                countryname.Id = id;
                appDbContex.Remove(countryname);
                appDbContex.SaveChangesAsync();
                return Ok(new { Message = "Record deleted successfully!" });
            }
            return BadRequest(new { Message = "Record not found!" });
        }

        [HttpPost("FindCountryById")]
        public ActionResult getCountrybyId(string id)
        {
            var countryname = appDbContex.Countries.Where(a => a.Id == id).FirstOrDefault();
            if (countryname != null)
            {
                return Ok(countryname);
            }
            return BadRequest(new { Message = "Record not found!" });
            }

     

    }
}