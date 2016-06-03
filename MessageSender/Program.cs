﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace MessageSender
{
    class MessageSender
    {
        public const string API_KEY = "AIzaSyDOmaN_SlHirOK0TSpeGWe737eo-6HOAkI";
        public const string MESSAGE = "Hello, Xamarin!";
        

        static void Main(string[] args)
        {
            var jGcmData = new JObject();
            var jData = new JObject();

            jData.Add("message", MESSAGE);
            jGcmData.Add("to", "/topics/global");
            //jGcmData.Add("to", "feyNemQXpNY:APA91bFdYfzvJJPIcehHaKkVjORW7WQhtOVW01QtAYV1xHojIY1Ec0rRhmd8Tx6bs7YWGXSBAIFm5I8tNv8YXXeE38OuULu12NVDLgrbgXxYJbl8-g-i50XWk-mBFsTMypv-czdXkDUS");
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
