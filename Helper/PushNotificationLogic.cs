using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SecureAfrica.Helper
{
    public class PushNotificationLogic
    {
        public PushNotificationLogic()
        {

        }
        public class Message
        {
            public string[] registration_ids { get; set; }
            public Notification notification { get; set; }
            public object data { get; set; }
        }
        public class Notification
        {
            public string title { get; set; }
            public string text { get; set; }
        }

        private static Uri FireBasePushNotificationsURL = new Uri("https://fcm.googleapis.com/fcm/send");
        private static string ServerKey = "AIzaSyBqVyZG2xWq3_2Tng4JCz9XXyL6KxY3_4Q";
        public async Task<bool> SendPushNotification(List<string> androidDeviceTocken, string title, string body, object data)
        {

            //deviceTokens: An array of strings, each string represents a FCM token provided by Firebase on each app - installation.This is going to be the list of app - installations that the notification is going to send.
            //title: It’s the bold section of a notification.
            //body: It represents “Message text” field of the Firebase SDK, this is the message you want to send to the users.
            //data: These is a dynamic object, it can be whatever you want because this object is going to be used as additional information you want to send to the app, it’s like hidden information. For example an action you want to execute when the user presses on the notification or an id of some product.


            string[] deviceTokens = new string[androidDeviceTocken.Count];
            deviceTokens = androidDeviceTocken.ToArray();

            bool sent = false;
            // var ServerKey = "AIzaSyBqVyZG2xWq3_2Tng4JCz9XXyL6KxY3_4Q";
            var messageInformation = new Message()
            {
                notification = new Notification()
                {
                    title = title,
                    text = body
                },
                data = data,
                registration_ids = deviceTokens
            };
            //Object to JSON STRUCTURE => using Newtonsoft.Json;
            string jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(messageInformation);

            // Create request to Firebase API
            var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationsURL);
            request.Headers.TryAddWithoutValidation("Authorization", "key =" + ServerKey);
            request.Content = new StringContent(jsonMessage, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                result = await client.SendAsync(request);
                sent = sent && result.IsSuccessStatusCode;
            }

            return sent;
        }
    }
}
//private static Uri FireBasePushNotificationsURL = new Uri("https://fcm.googleapis.com/fcm/send");
//private static string ServerKey = "AAAAvgqlqyM:APA91bGrOCHA2DYz8lq4iV7OhGAdV9qdLPril_PzMt9FAfI-ylb0oVtlG_Goa-vaElvRN8dK4Ux-PWo03OrXMKFtwcgdO3twYFFX_vz_jK6Lth3v-Cb_HaW3W6Ci_tUBL9JvqgMxYF5b";

///// <summary>
///// 
///// </summary>
///// <param name="deviceTokens">List of all devices assigned to a user</param>
///// <param name="title">Title of notification</param>
///// <param name="body">Description of notification</param>
///// <param name="data">Object with all extra information you want to send hidden in the notification</param>
///// <returns></returns>
//public static async Task<bool> SendPushNotification(string[] deviceTokens, string title, string body, object data)
//{
//    bool sent = false;

//    if (deviceTokens.Count() > 0)
//    {
//        //Object creation

//        var messageInformation = new Message()
//        {
//            notification = new Notification()
//            {
//                title = title,
//                text = body
//            },
//            data = data,
//            registration_ids = deviceTokens
//        };

//        //Object to JSON STRUCTURE => using Newtonsoft.Json;
//        string jsonMessage = JsonConvert.SerializeObject(messageInformation);

//        /*
//         ------ JSON STRUCTURE ------
//         {
//            notification: {
//                            title: "",
//                            text: ""
//                            },
//            data: {
//                    action: "Play",
//                    playerId: 5
//                    },
//            registration_ids = ["id1", "id2"]
//         }
//         ------ JSON STRUCTURE ------
//         */

//        //Create request to Firebase API
//        var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationsURL);

//        request.Headers.TryAddWithoutValidation("Authorization", "key=" + ServerKey);
//        request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

//        HttpResponseMessage result;
//        using (var client = new HttpClient())
//        {
//            result = await client.SendAsync(request);
//            sent = sent && result.IsSuccessStatusCode;
//        }
//    }

//    return sent;
//}