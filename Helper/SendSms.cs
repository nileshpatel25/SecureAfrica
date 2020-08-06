using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SecureAfrica.Helper
{
    public class SendSms
    {
        public void SendTextSms(string _Message, string _strMobile)
        {

            if (!string.IsNullOrEmpty(_strMobile.ToString()))
            {
                WebClient client = new WebClient();
                string to, message;
                to = _strMobile;
                message = _Message;
              
                client.OpenRead(baseURL);
            }
        }
    }
}
