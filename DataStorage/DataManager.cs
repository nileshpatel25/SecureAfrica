using Microsoft.AspNetCore.Identity;
using SecureAfrica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecureAfrica.Helper;

namespace SecureAfrica.DataStorage
{
    public class DataManager
    {
        public AppDbContex appDbContex { get; }
        FindUsers findUsers = new FindUsers();
        public DataManager()
        {

        }
        static int penidngcount = 0 ;
        static int confirmcount = 0;
        public DataManager(AppDbContex _appDbContex)
        {
            this.appDbContex = _appDbContex;
           
        }
      
    
        public static List<ChartModel> GetData()
        {
            var r = new Random();

         

            return new List<ChartModel>()
            {
                new ChartModel { Data = new List<int> { penidngcount }, Label = "Pending" },
                new ChartModel { Data = new List<int> { confirmcount }, Label = "Confirm" },
                new ChartModel { Data = new List<int> { r.Next(1, 40) }, Label = "Completed" },
                new ChartModel { Data = new List<int> { r.Next(1, 40) }, Label = "Cancel" }
            };
        }
    }
}
