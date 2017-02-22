using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using sas.Clases;
using System.Collections.Generic;

namespace sas
{
    [Service(Exported = false)]
    class UnRegistrationIntentService : IntentService
    {
        static object locker = new object();
        // Session Manager Class
        UserSessionManager session;
        string IPCONN = "";
        string access_token = "";
        string googleToken = "";
        public UnRegistrationIntentService() : base("UnRegistrationIntentService") { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info("UnRegistrationIntentService", "Calling Unsubcribe");
                lock (locker)
                {
                    session = new UserSessionManager(this);


                    //metodo unsubscribe was deprecated!!!!!!!
                    //recuperar la base para la coneccion
                    //googleToken = session.getGoogleToken();
                    //UbSubscribe(googleToken);


                    var instanceID = InstanceID.GetInstance(this);
                    instanceID.DeleteInstanceID();
                   // SendRegistrationToAppServer();

                    Log.Info("RegistrationIntentService", "GCM UnRegistration Token: " + googleToken);

                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
                return;
            }
            finally
            {
                session.logoutUser();
            }

          
        }
              

        void UbSubscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Unsubscribe(token, "/topics/global");
            
        }

        void SendRegistrationToAppServer()
        {

            session = new UserSessionManager(this);

            //recuperar la base para la coneccion
            IPCONN = session.getAccessConn();
            access_token = session.getAccessToken();
         
            // Add custom implementation here as needed.
            string result;

            var person = new Dictionary<string, string>
            {
                { "usuario" , session.getAccessUserId()},
                { "codMovil" ,session.getAccessIdmovil() },
                { "idRegistro" ,"" }
            };


            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;
                client.BaseAddress = new Uri(IPCONN);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);

                string url = string.Format("api/UsersApi/{0}", session.getAccessUserId());
                //var response = await client.PutAsync(url, content);
                var response = client.PutAsync(client.BaseAddress + url, new FormUrlEncodedContent(person)).Result;

                result = response.Content.ReadAsStringAsync().Result;

            }
            catch (Exception ex)
            {
                Log.Debug("RegistrationIntentService", "Failed to put a registration token");

                return;
            }

            Log.Debug("RegistrationIntentService", "Success to put a registration token");



        }
    }
}