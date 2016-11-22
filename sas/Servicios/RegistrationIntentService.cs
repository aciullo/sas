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
    class RegistrationIntentService : IntentService
    {
        static object locker = new object();
        // Session Manager Class
        UserSessionManager session;
        string IPCONN = "";
        string access_token = "";

        public RegistrationIntentService() : base("RegistrationIntentService") { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken(
                        sas.Core.Constantes.SENDER_ID, GoogleCloudMessaging.InstanceIdScope, null);

                    Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
                    SendRegistrationToAppServer(token);
                    Subscribe(token);
                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
                return;
            }
        }

        
        void SendRegistrationToAppServer(string token)
        {

            session = new UserSessionManager(this);

            //recuperar la base para la coneccion
            IPCONN = session.getAccessConn();
            access_token = session.getAccessToken();
            session.saveGoogleToken(token);
            // Add custom implementation here as needed.
            string result;

            var person = new Dictionary<string, string>
            {
                { "usuario" , session.getAccessUserId()},
                { "codMovil" ,session.getAccessIdmovil() },
                { "idRegistro" ,token }
            };

            //var person = new SimpledeviceUser();
            //person.usuario = session.getAccessUserId();
            //person.codMovil = session.getAccessIdmovil();
            //person.idRegistro = token;

            //var jsonResquest = JsonConvert.SerializeObject(person);
            //var content = new StringContent(jsonResquest, Encoding.UTF8, "text/json");

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

        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token, "/topics/global",null);
            pubSub.Unsubscribe(token, "/topics/global");
        }

        void UbSubscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Unsubscribe(token, "/topics/global");
        }
    }
}