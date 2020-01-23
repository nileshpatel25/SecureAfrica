using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureAfrica.DataModel;
using SecureAfrica.Models;
using SecureAfrica.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SecureAfrica.TimerFeatures;
using SecureAfrica.HubConfig;

namespace SecureAfrica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        public AppDbContex appDbContex { get; }
        private readonly UserManager<ApplicationUser> userManager;
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;
        private IHubContext<ChartHub> _hub;
        public IncidentController(AppDbContex _appDbContex, UserManager<ApplicationUser> userManager, IHubContext<NotifyHub, ITypedHubClient> hubContext, IHubContext<ChartHub> hub)
        {
            this.appDbContex = _appDbContex;
            this.userManager = userManager;
            this._hubContext = hubContext;
             _hub = hub;
        }

      


        [HttpPost]
        [Route("AddNewIncident")]
        public async Task<ActionResult> addnewIncident(IncidentRequest incidentRequest)
        {
            string incidentId = Guid.NewGuid().ToString();
            Incident incident = new Incident
            {

                Id = incidentId,
                UserAccountId = incidentRequest.UserAccountId,
                IncidentTypeId = incidentRequest.IncidentTypeId,
                IncidentPic = incidentRequest.IncidentPic,
                Address = incidentRequest.Address,
                AddressLine = incidentRequest.AddressLine,
                CoordinateX = incidentRequest.CoordinateX,
                CoordinateY = incidentRequest.CoordinateY,
                CustomerComments = incidentRequest.CustomerComments,
                Status = incidentRequest.Status,
                Deleted = false,
                CreatedAt = DateTime.Now
            };
            appDbContex.Incidents.Add(incident);
            await appDbContex.SaveChangesAsync();

            List<ApplicationUser> usersToNotify = new List<ApplicationUser>();
            FindUsers findUsers = new FindUsers(appDbContex, userManager);
            usersToNotify = findUsers.FindUsersToNotifybyCoordinateXY(incidentRequest.CoordinateX, incidentRequest.CoordinateY, incidentRequest.IncidentTypeId);

            PushNotificationLogic pushNotificationLogic = new PushNotificationLogic();
            //  string[] androidDeviceTocken;
            List<string> androidDeviceTocken = new List<string>();

            SendSms sendsms = new SendSms();
            var incidenttypeName = appDbContex.IncidentTypes.Where(a => a.Id == incidentRequest.IncidentTypeId).FirstOrDefault();
            string contactNumber = string.Empty;
            string location = "http://maps.google.com/?q=" + incidentRequest.CoordinateX + "," + incidentRequest.CoordinateY + "";
            string notificationMessage = string.Empty;
            foreach (var user in usersToNotify)
            {



                //if (user.Source == "Android")
                //{
                //    androidDeviceTocken.Add(user.PushTokenId);
                //}


                if (user.EmergencyContactNo != null)
                {
                    //Notification to Emergency Contact
                    contactNumber = user.InternationalPrefix.ToString() + user.PhoneNumber.ToString();
                    notificationMessage = "Here is the " + incidenttypeName.Name + " incident happen... please click here " + location;
                    //   sendsms.SendTextSms(notificationMessage, contactNumber);

                }
                else
                {
                    //Notification to Application Users
                    contactNumber = user.InternationalPrefix.ToString() + user.PhoneNumber.ToString();
                    notificationMessage = "Here is the " + incidenttypeName.Name + " incident happen... please click here " + location;
                    //  sendsms.SendTextSms(notificationMessage, contactNumber);
                }

                NotifiedUser notifiedUser = new NotifiedUser
                {
                    Id = Guid.NewGuid().ToString(),
                    IncidentId = incidentId,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now
                };
                appDbContex.NotifiedUsers.Add(notifiedUser);
                await appDbContex.SaveChangesAsync();

                IncidentUserMessage incidentUserMessage = new IncidentUserMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    IncidentId = incidentId,
                    UserId = user.Id,
                    Status = incidentRequest.Status,
                    StatusMessage = notificationMessage,
                    CreatedAt = DateTime.Now
                };

                appDbContex.IncidentUserMessages.Add(incidentUserMessage);
                await appDbContex.SaveChangesAsync();
            }

            try
            {
                //  androidDeviceTocken = null;
                //  androidDeviceTocken.Add("f4e9GJxSvYA:APA91bGkImijMYelYhFCqFTE6qDzEfzeEdM6H3Q1XwxxCDvYWZGdyviRGtPSdTcyeXy4787JPwfb04pFNWo5dXIc420EVZEQ15UtHqTCAn8kk8zdAJ8pgRLLMNbKkJ1dfR5ABMoMJd71");
                //  androidDeviceTocken.Add("ccSVgC04gaM:APA91bGt8rDg-1CyG5N9pxW5aXWs9x4jpf6rXYXRu0usnaiMzgfosr5Guv89iJbiHBvUcOYkGf2RIJBx_-jtK_76bZwk__d3Xn94TSXLmaC8rs9GEnIvX5AOldPXqp1EiUvIrt1zfQcr");

                androidDeviceTocken.Add("dndndqNSHbs:APA91bFrO7Au5DvoYIgFaWY1S7PLAzzcwZ9EcuwjKvqFBdM-733zwDKCWnT5JZ9FcSVsUb1JwYUWCElXmFpgd6BXkTcUn9ejhvrwvB0eIC9Mpn4komqfT_APS2TaX9ZtZ_a_TbjfFswH");
                if (androidDeviceTocken != null)
                {
                    //     Myobject myobj = new Myobject
                    //    {
                    //        Name = "Bhavin",
                    //        City = "vapi"
                    //    };

                    var myobj = new { Name = "Bhavin", City = "Vapi" };


                    await pushNotificationLogic.SendPushNotification(androidDeviceTocken, "SecureAfrica", notificationMessage, myobj);
                }

            }
            catch (Exception ex)
            {

            }
            await _hubContext.Clients.All.BroadcastMessage("success", "this is our msg");
            return Ok(new { Message = "Incident added successfully !" });
        }

        [HttpPost]
        [Route("UpdateIncidentStatus")]
        public async Task<ActionResult> updateIncidentStatus(UpdateIncidentStatusRequest updateIncidentStatusRequest)
        {
            Incident incident = appDbContex.Incidents.Where(a => a.Id == updateIncidentStatusRequest.Id).FirstOrDefault();
            if (incident != null)
            {
                if (incident.EmergencyAccountId == null || incident.EmergencyAccountId == updateIncidentStatusRequest.EmergencyAccountId)
                {
                    incident.EmergencyAccountId = updateIncidentStatusRequest.EmergencyAccountId;
                    incident.Status = updateIncidentStatusRequest.Status;
                    incident.UpdatedAt = DateTime.Now;
                    await appDbContex.SaveChangesAsync();

                    List<ApplicationUser> usersToNotify = new List<ApplicationUser>();
                    FindUsers findUsers = new FindUsers(appDbContex, userManager);
                    usersToNotify = findUsers.FindUsersToNotifybyCoordinateXY(incident.CoordinateX, incident.CoordinateY, incident.IncidentTypeId);

                    SendSms sendsms = new SendSms();
                    string contactNumber = string.Empty;
                    var incidenttypeName = appDbContex.IncidentTypes.Where(a => a.Id == incident.IncidentTypeId).FirstOrDefault();
                    foreach (var user in usersToNotify)
                    {
                        NotifiedUser notifiedUser = appDbContex.NotifiedUsers.Where(a => a.IncidentId == updateIncidentStatusRequest.Id && a.UserId == user.Id).FirstOrDefault();

                        if (notifiedUser == null)
                        {
                            NotifiedUser addNotifiedUser = new NotifiedUser
                            {
                                Id = Guid.NewGuid().ToString(),
                                IncidentId = updateIncidentStatusRequest.Id,
                                UserId = user.Id,
                                CreatedAt = DateTime.Now
                            };

                            appDbContex.NotifiedUsers.Add(addNotifiedUser);
                            await appDbContex.SaveChangesAsync();
                        }
                    }

                    List<NotifiedUser> lstnotifiedUsers = appDbContex.NotifiedUsers.Where(a => a.IncidentId == updateIncidentStatusRequest.Id).ToList();
                    List<ApplicationUser> usersToNotify1 = new List<ApplicationUser>();
                    foreach (var user in lstnotifiedUsers)
                    {
                        ApplicationUser appUser = userManager.Users.Where(a => a.Id == user.UserId).FirstOrDefault();
                        if (appUser != null)
                        {
                            usersToNotify1.Add(appUser);
                        }
                    }
                    string notificationMessage = string.Empty;
                    string location = "http://maps.google.com/?q=" + incident.CoordinateX + "," + incident.CoordinateY + "";
                    foreach (var user in usersToNotify1)
                    {


                        if (user.EmergencyContactNo != null)
                        {
                            //Notification to Emergency Contact
                            contactNumber = user.InternationalPrefix.ToString() + user.PhoneNumber.ToString();
                            notificationMessage = "Here is the " + incidenttypeName.Name + " incident happen... please click here " + location;
                            sendsms.SendTextSms(notificationMessage, contactNumber);


                        }
                        else
                        {
                            //Notification to User
                            contactNumber = user.InternationalPrefix.ToString() + user.PhoneNumber.ToString();
                            notificationMessage = "Here is the " + incidenttypeName.Name + " incident happen... please click here " + location;
                            sendsms.SendTextSms(notificationMessage, contactNumber);
                        }




                        IncidentUserMessage incidentUserMessage = new IncidentUserMessage
                        {
                            Id = Guid.NewGuid().ToString(),
                            IncidentId = updateIncidentStatusRequest.Id,
                            UserId = user.Id,
                            Status = updateIncidentStatusRequest.Status,
                            StatusMessage = notificationMessage,
                            CreatedAt = DateTime.Now
                        };

                        appDbContex.IncidentUserMessages.Add(incidentUserMessage);
                        await appDbContex.SaveChangesAsync();

                    }
                }

                return Ok(new { Message = "Incident Updated successfully !" });
            }
            return BadRequest(new { Message = "Incident Not Found !" });

        }

        [HttpPost]
        [Route("getIncidentByEmAccountIdbyType")]
        public ActionResult getIncidentByEmAccountIdbyType(string id, string typeId)
        {
            var obj = (from trns in appDbContex.Incidents
                       join st in appDbContex.Users
                       on trns.UserAccountId equals st.Id
                       join pt in appDbContex.IncidentTypes
                       on trns.IncidentTypeId equals pt.Id
                       select new
                       {
                           trns.Id,
                           trns.UserAccountId,
                           trns.EmergencyAccountId,
                           trns.IncidentTypeId,
                           trns.Deleted,
                           trns.CustomerComments,
                           trns.AddressLine,
                           trns.Address,
                           trns.CoordinateX,
                           trns.CoordinateY,
                           trns.CreatedAt,
                           trns.Status,
                           st.Name,
                           st.Email,
                           st.Source,
                           EmergencyOragnization = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().OrganizationName,
                           EmergencyContactName = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().Name,
                           EmergencyContactPhone = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().PhoneNumber,
                           EmergencyContactNo = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().EmergencyContactNo,
                           st.InternationalPrefix,
                           st.PhoneNumber,
                           st.AdditionalDetails,
                           IncidentTypename = pt.Name
                       }).Where(a => a.Deleted == false && a.EmergencyAccountId== id && a.IncidentTypeId==typeId).ToList();


            return Ok(obj);
        }




        [HttpPost]
        [Route("getIncidentByincidentId")]
        public ActionResult getIncidentDetailsbyId(string id)
        {
            var obj = (from trns in appDbContex.Incidents
                       join st in appDbContex.Users
                       on trns.UserAccountId equals st.Id
                       join pt in appDbContex.IncidentTypes
                       on trns.IncidentTypeId equals pt.Id
                       select new
                       {
                           trns.Id,
                           trns.UserAccountId,
                           trns.EmergencyAccountId,
                           trns.IncidentTypeId,
                           trns.Deleted,
                           trns.CustomerComments,
                           trns.AddressLine,
                           trns.Address,
                           trns.CoordinateX,
                           trns.CoordinateY,
                           trns.CreatedAt,
                           trns.Status,
                           st.Name,
                           st.Email,
                           st.Source,
                           EmergencyOragnization = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().OrganizationName,
                           EmergencyContactName = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().Name,
                           EmergencyContactPhone = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().PhoneNumber,
                           EmergencyContactNo = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().EmergencyContactNo,
                           st.InternationalPrefix,
                           st.PhoneNumber,
                           st.AdditionalDetails,
                           IncidentTypename = pt.Name
                       }).Where(a => a.Deleted == false && a.Id == id).ToList();


            return Ok(obj);
        }
        [HttpGet]
        [Route("getAllIncidentHistory")]
        public ActionResult getAllIncident()
        {

            var obj = (from trns in appDbContex.Incidents
                       join st in appDbContex.Users
                       on trns.UserAccountId equals st.Id
                       join pt in appDbContex.IncidentTypes
                       on trns.IncidentTypeId equals pt.Id
                       select new
                       {
                           trns.Id,
                           trns.UserAccountId,
                           trns.EmergencyAccountId,
                           trns.IncidentTypeId,
                           trns.Deleted,
                           trns.CustomerComments,
                           trns.AddressLine,
                           trns.Address,
                           trns.CoordinateX,
                           trns.CoordinateY,
                           trns.CreatedAt,
                           trns.Status,
                           st.Name,
                           st.Email,
                           st.Source,
                           EmergencyOragnization = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().OrganizationName,
                           EmergencyContactName = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().Name,
                           EmergencyContactPhone = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().PhoneNumber,
                           EmergencyContactNo = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().EmergencyContactNo,
                           st.InternationalPrefix,
                           st.PhoneNumber,
                           st.AdditionalDetails,
                           IncidentTypename = pt.Name
                       }).Where(a => a.Deleted == false).ToList();


            return Ok(obj);
        }
        [HttpPost]
        [Route("allincidentbyaccountid")]
        public ActionResult getallIncidentbyaccountid(string id)
        {
            List<Incident> IncidentHistorybyAccountid = new List<Incident>();

            IncidentHistorybyAccountid = appDbContex.Incidents.Where(a => a.UserAccountId == id && a.Deleted == false).ToList();
            var obj = (from trns in appDbContex.Incidents
                       join pt in appDbContex.IncidentTypes on trns.IncidentTypeId equals pt.Id
                       join st in appDbContex.Users on new
                       {
                           accountid = id

                       }
  equals new
  {
      accountid = st.Id,

  }
                       where trns.UserAccountId == id

                       select new
                       {
                           trns.Id,
                           trns.UserAccountId,
                           trns.EmergencyAccountId,
                           trns.IncidentTypeId,
                           trns.Deleted,
                           trns.CustomerComments,
                           trns.AddressLine,
                           trns.Address,
                           trns.CoordinateX,
                           trns.CoordinateY,
                           trns.CreatedAt,
                           trns.Status,
                           st.Name,
                           st.Email,
                           st.Source,
                           EmergencyOragnization = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().OrganizationName,
                           EmergencyContactName = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().Name,
                           EmergencyContactPhone = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().PhoneNumber,
                           EmergencyContactNo = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().EmergencyContactNo,
                           st.InternationalPrefix,
                           st.PhoneNumber,
                           st.AdditionalDetails,
                           IncidentTypename = pt.Name
                       }).Where(a => a.Deleted == false).ToList();
            return Ok(obj);
        }

        [HttpPost]
        [Route("allincidentbytypeid")]
        public ActionResult getallIncidentbyTypeId(string id)
        {
            List<Incident> IncidentHistorybyAccountid = new List<Incident>();

            IncidentHistorybyAccountid = appDbContex.Incidents.Where(a => a.IncidentTypeId == id && a.Deleted == false).ToList();
            var obj = (from trns in appDbContex.Incidents
                       join st in appDbContex.Users
                     on trns.UserAccountId equals st.Id
                       // join pt in context.Incident on trns.Accountid equals pt.incidentID
                       join pt in appDbContex.IncidentTypes on new
                       {
                           incidentid = id

                       }
  equals new
  {
      incidentid = pt.Id,

  }
                       where trns.IncidentTypeId == id

                       select new
                       {
                           trns.Id,
                           trns.UserAccountId,
                           trns.EmergencyAccountId,
                           trns.IncidentTypeId,
                           trns.Deleted,
                           trns.CustomerComments,
                           trns.AddressLine,
                           trns.Address,
                           trns.CoordinateX,
                           trns.CoordinateY,
                           trns.CreatedAt,
                           trns.Status,
                           st.Name,
                           st.Email,
                           st.Source,
                           EmergencyOragnization = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().OrganizationName,
                           EmergencyContactName = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().Name,
                           EmergencyContactPhone = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().PhoneNumber,
                           EmergencyContactNo = appDbContex.Users.Where(a => a.Id == trns.EmergencyAccountId).FirstOrDefault().EmergencyContactNo,
                           st.InternationalPrefix,
                           st.PhoneNumber,
                           st.AdditionalDetails,
                           IncidentTypename = pt.Name
                       }).Where(a => a.Deleted == false).ToList();
            return Ok(obj);

        }

        [HttpGet]
        [Route("statusChart")]
        public ActionResult getStatusData()
        {

            List<Incident> incident = new List<Incident>();
            List<Incident> listObjects = appDbContex.Incidents.GroupBy(a => a.Status).Select(a => a.FirstOrDefault()).ToList();

            var obj = (from newobj in listObjects.Where(a => a.Deleted == false)
                       select new
                       {
                           newobj.Status,
                           count = appDbContex.Incidents.Where(a => a.Status == newobj.Status).Count(),
                           newobj.Deleted

                       }).Where(a => a.Deleted == false).ToList();

            return Ok(obj);
        }


        public void getpendingIndicent()
        {
            int count = 0;
            count = appDbContex.Incidents.Where(a => a.Status == "Pending").Count();

        }



        [HttpPost]
        [Route("deleteIncidentbyId")]
        public async Task<ActionResult> deleteIncident(string id)
        {

            Incident incidents = appDbContex.Incidents.Where(a => a.Id == id).FirstOrDefault();
            if (incidents != null)
            {
                incidents.Deleted = true;
                incidents.UpdatedAt = DateTime.Now;
                await appDbContex.SaveChangesAsync();

                return Ok(new { Message = "Incident deleted successfully !" });
            }
            return BadRequest(new { Message = "Incident Not Found !" });
        }

      
        [HttpGet]
        [Route("CountTodayIncident")]
        public ActionResult getCountTodayIncident()
        {
            int? count = 0;
            count = appDbContex.Incidents.Where(a => Convert.ToDateTime(a.CreatedAt.Date) == Convert.ToDateTime(DateTime.Now.Date)).Count();
            if (count == null)
            {
                count = 0;
            }
            return Ok(count);
        }

        [HttpGet]
        [Route("CountPendingIncident")]
        public ActionResult CountPendingIncident()
        {
            int? count = 0;
            count = appDbContex.Incidents.Where(a => a.Status == "Pending" && Convert.ToDateTime(a.CreatedAt.Date) == Convert.ToDateTime(DateTime.Now.Date)).Count();
            if (count == null)
            {
                count = 0;
            }
            return Ok(count);
        }
        [HttpGet]
        [Route("CountConfirmIncident")]
        public ActionResult CountConfirmIncident()
        {
            int? count = 0;
            count = appDbContex.Incidents.Where(a => a.Status == "Confirm" && Convert.ToDateTime(a.CreatedAt.Date) == Convert.ToDateTime(DateTime.Now.Date)).Count();
            if (count == null)
            {
                count = 0;
            }
            return Ok(count);
        }

        //Procesing
        [HttpGet]
        [Route("CountProcesingIncident")]
        public ActionResult CountProcesingIncident()
        {
            int? count = 0;
            count = appDbContex.Incidents.Where(a => a.Status == "Procesing" && Convert.ToDateTime(a.CreatedAt.Date) == Convert.ToDateTime(DateTime.Now.Date)).Count();
            if (count == null)
            {
                count = 0;
            }
            return Ok(count);
        }

        //Complated
        [HttpGet]
        [Route("CountComplatedIncident")]
        public ActionResult CountComplatedIncident()
        {
            int? count = 0;
            count = appDbContex.Incidents.Where(a => a.Status == "Complated" && Convert.ToDateTime(a.CreatedAt.Date) == Convert.ToDateTime(DateTime.Now.Date)).Count();
            if (count == null)
            {
                count = 0;
            }
            return Ok(count);
        }
        //Cancel
        [HttpGet]
        [Route("CountCancelIncident")]
        public ActionResult CountCancelIncident()
        {
            int? count = 0;
            count = appDbContex.Incidents.Where(a => a.Status == "Cancel" && Convert.ToDateTime(a.CreatedAt.Date) == Convert.ToDateTime(DateTime.Now.Date)).Count();
            if (count == null)
            {
                count = 0;
            }
            return Ok(count);
        }
    }

    
   // count = appDbContex.Incidents.Where(a => a.Status == "Pending").Count();

  

   


}