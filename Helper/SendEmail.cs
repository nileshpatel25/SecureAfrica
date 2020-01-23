using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SecureAfrica.Helper
{
    public class SendEmail
    {

        public void sendemail(string Emailid)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add(Emailid.ToString());
            mail.From = new MailAddress("SecureAfr@gmail.com");
            mail.From = new System.Net.Mail.MailAddress("nileshpatel2533@gmail.com", "Registration Successful (Secure Africa)");
            mail.Subject = "Thank you for registering (Secure Africa)";
            //  string msg = li.emailid;
            mail.IsBodyHtml = true;
          //  mail.Body = "testing email";
            string htmlString = @"<html>
	                      <body>
	                      <p>Dear,</p>

	                      <p>Thank you for registering to the Secure Africa. </p>

	                      <p>Sincerely,<br>Secure africa team</br></p>

	                      </body>

	                      </html>

	                     ";


            mail.Body = htmlString;


            //relay-hosting.secureserver.net

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("SecureAfr@gmail.com", "SecAfr123");
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }


    }
}
