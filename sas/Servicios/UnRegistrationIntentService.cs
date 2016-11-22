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

                    //recuperar la base para la coneccion
                    googleToken = session.getGoogleToken();
                   

                    Log.Info("RegistrationIntentService", "GCM UnRegistration Token: " + googleToken);
                   
                    UbSubscribe(googleToken);

                    session.logoutUser();
                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
                return;
            }
        }
              

        void UbSubscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Unsubscribe(token, "/topics/global");
            
        }
    }
}