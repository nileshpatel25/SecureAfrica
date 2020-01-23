using Microsoft.AspNetCore.Identity;
using SecureAfrica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAfrica.Helper
{
    public class FindUsers
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AppDbContex appDbContex { get; }
        //        public static UserManager<ApplicationUser> userManager;

        public FindUsers()
        {

        }
        public FindUsers(AppDbContex _appDbContex, UserManager<ApplicationUser> userManager)
        {
            this.appDbContex = _appDbContex;
            this.userManager = userManager;
            //  this.userManager = _userManager;
        }
        
        

       public static int penidngcount()
        {
            return 2;
        }


        public List<ApplicationUser> FindUsersToNotifybyCoordinateXY(float CoordinateX, float CoordinateY, string incidentTypeId)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            // applicationUsers = appdbcontext.Users.ToList();
            // UserManager userManager;
            
            applicationUsers = userManager.Users.ToList();
            List<ApplicationUser> usersToNotify = new List<ApplicationUser>();
            foreach (var user in applicationUsers)
            {
                if(user.IncidentTypeId==null)
                { 
                double lat1 = System.Convert.ToDouble(CoordinateX);
                double lon1 = System.Convert.ToDouble(CoordinateY);
                double lat2 = System.Convert.ToDouble(user.CoordinateX);
                double lon2 = System.Convert.ToDouble(user.CoordinateY);
                char unit = 'K';
                var userdistance = distance(lat1, lon1, lat2, lon2, unit);
                if (userdistance <= 20)
                {
                    usersToNotify.Add(user);
                }
                }
                else if(user.IncidentTypeId == incidentTypeId)
                {
                    double lat1 = System.Convert.ToDouble(CoordinateX);
                    double lon1 = System.Convert.ToDouble(CoordinateY);
                    double lat2 = System.Convert.ToDouble(user.CoordinateX);
                    double lon2 = System.Convert.ToDouble(user.CoordinateY);
                    char unit = 'K';
                    var userdistance = distance(lat1, lon1, lat2, lon2, unit);
                    if (userdistance <= 20)
                    {
                        usersToNotify.Add(user);
                    }
                }


            }
            int emergencyCount = 0;
            emergencyCount = usersToNotify.Where(a => a.EmergencyContactNo != null && a.IncidentTypeId == incidentTypeId.ToString()).Count();

            //var Distance = new ApplicationUser();
            //Distance.Name = "Distance";

            //EmergencyusersToNotify.Add(Distance);

            if (emergencyCount == 0)
            {
                List<ApplicationUser> EmergencyusersToNotify = new List<ApplicationUser>();
                EmergencyusersToNotify = userManager.Users.Where(a => a.EmergencyContactNo != null && a.IncidentTypeId == incidentTypeId.ToString()).ToList();



                foreach (var user in EmergencyusersToNotify)
                {

                    double lat1 = System.Convert.ToDouble(CoordinateX);
                    double lon1 = System.Convert.ToDouble(CoordinateY);
                    double lat2 = System.Convert.ToDouble(user.CoordinateX);
                    double lon2 = System.Convert.ToDouble(user.CoordinateY);
                    char unit = 'K';
                    var userdistance = distance(lat1, lon1, lat2, lon2, unit);
                    user.Distance = userdistance;
                }

                EmergencyusersToNotify = EmergencyusersToNotify.OrderBy(a => a.Distance).Take(5).ToList();

                usersToNotify.AddRange(EmergencyusersToNotify);
            }

            return usersToNotify;

        }

        private static double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double deg2rad(double deg)
        {
            
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }


    }
}
