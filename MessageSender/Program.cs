using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace MessageSender
{
    class MessageSender
    {
        public const string API_KEY = "AIzaSyA9B9EyPB3ywCbhExZDMOWy0MxBG7479ws";
        public const string MESSAGE = "Hello mundo!";
        

        static void Main(string[] args)
        {
            var jGcmData = new JObject();
            var jData = new JObject();

            jData.Add("message", MESSAGE);
           jGcmData.Add("to", "/topics/global");
           // jGcmData.Add("to", "eMfglYQn4H0:APA91bH4P0kHehcxny4ilhaDehCuSk6MBp2ilXVJ4xFPJu8zIey8RrDcoiUN0SdEqRHhvEkzoiIFyoMkJDaS6kh4IK7ydgvyQLnfyrU-uZu-Hza3gHu4hgmXJYX-GJONSFtbfFgweffD");
            jGcmData.Add("data", jData);

            var url = new Uri("https://gcm-http.googleapis.com/gcm/send");
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        "Authorization", "key=" + API_KEY);

                    Task.WaitAll(client.PostAsync(url,
                        new StringContent(jGcmData.ToString(), Encoding.Default, "application/json"))
                            .ContinueWith(response =>
                            {
                                Console.WriteLine(response.Status);
                                Console.WriteLine(response.Result);
                                Console.WriteLine("Message sent: check the client device notification tray.");
                            }));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to send GCM message:");
                Console.Error.WriteLine(e.StackTrace);
            }
        }
    }
}
