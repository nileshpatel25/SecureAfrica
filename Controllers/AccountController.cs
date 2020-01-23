using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureAfrica.DataModel;
using SecureAfrica.Models;
using SecureAfrica.Helper;
namespace SecureAfrica.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AppDbContex appDbContex { get; }

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, AppDbContex _appDbContex)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.appDbContex = _appDbContex;
        }


        //[AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {

            var user = new ApplicationUser
            {
                UserName = registerRequest.PhoneNumber,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                Name = registerRequest.Name,
                Country = registerRequest.Country,
                InternationalPrefix = registerRequest.InternationalPrefix,
                Address = registerRequest.Address,
                CoordinateX = registerRequest.CoordinateX,
                CoordinateY = registerRequest.CoordinateY,
                Source = registerRequest.Source,
                PushTokenId = registerRequest.PushTokenId,
                IsVolunteer = registerRequest.IsVolunteer,
                EmergencyContactNo = registerRequest.EmergencyContactNo,
                OrganizationName = registerRequest.OrganizationName,
                IncidentTypeId = registerRequest.IncidentTypeId,
                AdditionalDetails = registerRequest.AdditionalDetails

            };

            var result = await userManager.CreateAsync(user, registerRequest.Password);
            if (result.Succeeded)
            {
                SendEmail sendEmail = new SendEmail();
                //sendEmail.sendemail(registerRequest.Email);
                if (user.Source == "Android" || user.Source == "iOS" || user.Source == "Web")
                {
                    if (registerRequest.EmergencyContactNo != null)
                    {
                        var Ec_role = await roleManager.FindByNameAsync("Ec_Admin");
                        if (Ec_role != null)
                        {
                            IdentityResult roleresult = null;
                            roleresult = await userManager.AddToRoleAsync(user, Ec_role.Name);
                            if (!roleresult.Succeeded)
                            {
                                return BadRequest(new { Message = "New user Created But Role has been not assigned !" });
                            }
                        }
                        else
                        {
                            return BadRequest(new { Message = "New user Created But Role not found !" });
                        }

                    }
                    else
                    {
                        var role = await roleManager.FindByNameAsync("User");
                        if (role != null)
                        {
                            IdentityResult roleresult = null;
                            roleresult = await userManager.AddToRoleAsync(user, role.Name);
                            if (!roleresult.Succeeded)
                            {
                                return BadRequest(new { Message = "New user Created But Role has been not assigned !" });
                            }
                        }
                        else
                        {
                            return BadRequest(new { Message = "New user Created But Role not found !" });
                        }
                    }

                }
                if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                {
                    return Ok(new { Message = "New user Created!" });
                }
                else
                {
                    LoginRequest loginRequest = new LoginRequest
                    {
                        UserName = user.UserName,
                        Password = registerRequest.Password,
                        RememberMe = false
                    };
                    var returnResult = await Login(loginRequest);
                    //  await signInManager.SignInAsync(user, isPersistent: true);
                    return Ok(returnResult);
                }
            }
            List<string> errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }
            return BadRequest(new { message = errors });

        }

        [HttpPost("updateEmergencyContact")]
        public async Task<IActionResult> updateEmergencyContact(RegisterRequest registerRequest)
        {
            var phonenumber = userManager.Users.Where(a => a.PhoneNumber == registerRequest.PhoneNumber && a.Id != registerRequest.id).SingleOrDefault();
            if (phonenumber == null)
            {
                var user = userManager.Users.Where(a => a.Id == registerRequest.id).SingleOrDefault();
                if (user != null)
                {
                    user.IncidentTypeId = registerRequest.IncidentTypeId;
                    user.Name = registerRequest.Name;
                    user.OrganizationName = registerRequest.OrganizationName;
                    user.Email = registerRequest.Email;
                    user.Country = registerRequest.Country;
                    user.InternationalPrefix = registerRequest.InternationalPrefix;
                    user.PhoneNumber = registerRequest.PhoneNumber;
                    user.EmergencyContactNo = registerRequest.EmergencyContactNo;
                    user.Address = registerRequest.Address;
                    user.CoordinateX = registerRequest.CoordinateX;
                    user.CoordinateY = registerRequest.CoordinateY;
                    user.AddressLine = registerRequest.AddressLine;
                    user.AdditionalDetails = registerRequest.AdditionalDetails;
                    await userManager.UpdateAsync(user);

                    return Ok(new { message="Data Updated Successfully!"});

                }
            }
            return BadRequest(new { message = "PhoneNumebr Already Exists!" });

        }

        [HttpPost]
        [Route("deletebyId")]
        public ActionResult deletbyId(string id)
        {
            var user = userManager.Users.Where(a => a.Id == id).FirstOrDefault();
            if (user != null)
            {
                user.Id = id;
                userManager.DeleteAsync(user);
              
                return Ok(new { Message = "Record deleted successfully!" });
            }
            return BadRequest(new { Message = "Record not found!" });
        }

        [HttpGet]
        [Route("CountUser")]
        public ActionResult getUserCount()
        {
            int? count = 0;
            count = userManager.Users.Where(a => a.EmergencyContactNo == null && a.IncidentTypeId == null).Count();
            if (count == null)
            {
                count = 0;
            }
            return Ok(count);
        }

        [HttpGet]
        [Route("CountEmgencyContact")]
        public ActionResult getemergencyCount()
        {
            int? count = 0;
            count = userManager.Users.Where(a => a.EmergencyContactNo != null && a.IncidentTypeId != null).Count();
            if (count == null)
            {
                count = 0;
            }
            return Ok(count);
        }
        [HttpGet]
        [Route("getAllUser")]
        public ActionResult getAllUser()
        {

            List<ApplicationUser> EcapplicationUsers = new List<ApplicationUser>();
            // applicationUsers = appdbcontext.Users.ToList();
            // UserManager userManager;

            EcapplicationUsers = userManager.Users.Where(a => a.EmergencyContactNo == null && a.IncidentTypeId == null).ToList();
            //List<ApplicationUser> allecUser = 
            return Ok(EcapplicationUsers);
        }

        [HttpGet]
        [Route("getAllEmegencyContact")]
        public ActionResult getAllEmegencyContacts()
        {

            List<ApplicationUser> EcapplicationUsers = new List<ApplicationUser>();
            // applicationUsers = appdbcontext.Users.ToList();
            // UserManager userManager;

            EcapplicationUsers = userManager.Users.Where(a => a.EmergencyContactNo != null && a.IncidentTypeId != null).ToList();

            var obj = (from user in userManager.Users
                       join type in appDbContex.IncidentTypes on user.IncidentTypeId equals type.Id
                       select new
                       {
                           user.Name,
                           user.AdditionalDetails,
                           user.Address,
                           user.AddressLine,
                           user.CoordinateX,
                           user.CoordinateY,
                           user.Country,
                           user.Distance,
                           user.Email,
                           user.EmergencyContactNo,
                           user.Id,
                           user.IncidentTypeId,
                           user.InternationalPrefix,
                           user.IsVolunteer,
                           user.OrganizationName,
                           user.PhoneNumber,
                           user.ProfilePic,
                           user.PushTokenId,
                           user.Source,
                           user.UserName,
                           incidenttypename = type.Name

                       }).ToList();
            //List<ApplicationUser> allecUser = 
            return Ok(obj);
        }

        [HttpPost]
        [Route("getUserByUserId")]
        public ActionResult getUserbyUserId(string Id)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            applicationUsers = userManager.Users.Where(a => a.Id == Id).ToList();
            return Ok(applicationUsers);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var result = await signInManager.PasswordSignInAsync(loginRequest.UserName, loginRequest.Password, loginRequest.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(loginRequest.UserName);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };
                var signinkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecurityKey"));
                var tocken = new JwtSecurityToken(
                   issuer: "http://saveafrica.com",
                   audience: "http://saveafrica.com",
                   expires: DateTime.Now.AddHours(1),
                   claims: claims,
                   signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signinkey, SecurityAlgorithms.HmacSha256)
                );
                var role = await userManager.GetRolesAsync(user);
                // return Ok(user);
                return Ok(new
                {
                    authonticationTocken = new JwtSecurityTokenHandler().WriteToken(tocken),
                    token = new JwtSecurityTokenHandler().WriteToken(tocken),
                    expiration = tocken.ValidTo,
                    role = role,
                    userId = user.Id
                });
            }
            else
            {
                return BadRequest(new { Message = "Invalid Username Or Passwrord!" });
            }
        }


        [HttpPost("createrole")]
        public async Task<IActionResult> CreateRole(CreateRoleRequest createRoleRequest)
        {
            IdentityRole role = new IdentityRole(createRoleRequest.RoleName);
            IdentityResult result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok(role);
            }
            return BadRequest(new { Message = "Role Already Existed!" });
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok(new { Message = "Logout Successfull !" });
        }
    }
}