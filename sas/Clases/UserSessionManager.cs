using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Java.Util.Prefs;

namespace sas.Clases
{
    public class UserSessionManager
    {
        private ISharedPreferences mSharedPrefs;
        private ISharedPreferencesEditor mPrefsEditor;
        private Context mContext;
        
        

        public static String PREFERENCE_USER = "PREFERENCE_USER";
        public static String PREFERENCE_IDMOVIL = "PREFERENCE_IDMOVIL";
        public static String PREFERENCE_CONEXION = "PREFERENCE_CONEXION";
        // All Shared Preferences Keys
        private static String IS_LOGIN = "IsLoggedIn";

        public UserSessionManager(Context context)
        {
            this.mContext = context;
            mSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            mPrefsEditor = mSharedPrefs.Edit();
            if (string.IsNullOrEmpty(getAccessConn()))
            {
                saveAccessIP("http://192.168.0.102:88");
            }
            


        }

        public void saveAccessKey(string key, string key2)
        {
            mPrefsEditor.PutString(PREFERENCE_USER, key);
            mPrefsEditor.PutString(PREFERENCE_IDMOVIL, key2);
            mPrefsEditor.Commit();
        }

        public void saveAccessIP(string conn)
        {
            mPrefsEditor.PutString(PREFERENCE_CONEXION, conn);
            mPrefsEditor.Commit();
        }

        public string getAccessKey()
        {
            return mSharedPrefs.GetString(PREFERENCE_USER, "");
        }
        public string getAccessIdmovil()
        {
            return mSharedPrefs.GetString(PREFERENCE_IDMOVIL, "");
        }

        public string getAccessConn()
        {
            return mSharedPrefs.GetString(PREFERENCE_CONEXION, "");
        }

        /**
     * Create login session
     * */
        public void createLoginSession(String user, String idmovil)
        {
            // Storing login value as TRUE
            mPrefsEditor.PutBoolean(IS_LOGIN, true);

            // Storing name in pref
            mPrefsEditor.PutString(PREFERENCE_USER, user);

            // Storing email in pref
            mPrefsEditor.PutString(PREFERENCE_IDMOVIL, idmovil);

            // commit changes
            mPrefsEditor.Commit();
        }

        /**
    * Quick check for login
    * **/
        // Get Login State
        public bool isLoggedIn()
        {
            return mSharedPrefs.GetBoolean(IS_LOGIN, false);
        }

        /**
    * Check login method wil check user login status
    * If false it will redirect user to login page
    * Else won't do anything
    * */
        public void checkLogin()
        {
            // Check login status
            if (!this.isLoggedIn())
            {
                // user is not logged in redirect him to Login Activity
                Intent i = new Intent(mContext, typeof(MainActivity));
                // Closing all the Activities
                i.SetFlags(ActivityFlags.ClearTop);

                // Add new Flag to start new Activity
                i.SetFlags(ActivityFlags.NewTask);

                // Staring Login Activity
                mContext.StartActivity(i);
            }
        }

        /**
   * Get stored session data
   * */
        public Dictionary<string, string> getUserDetails()
        {
            Dictionary<string, string> user = new Dictionary<string, string>();
            // user
            user.Add(PREFERENCE_USER, mSharedPrefs.GetString(PREFERENCE_USER, null));
            // user idmovil
            user.Add(PREFERENCE_IDMOVIL, mSharedPrefs.GetString(PREFERENCE_IDMOVIL, null));
            // return user
            return user;
        }

        /**
    * Clear session details
    * */
        public void logoutUser()
        {
            // Clearing all data from Shared Preferences
            mPrefsEditor.Clear();
            mPrefsEditor.Commit();

            // After logout redirect user to Loing Activity
            Intent i = new Intent(mContext, typeof(MainActivity));

            // Closing all the Activities
            i.SetFlags(ActivityFlags.ClearTop);

            // Add new Flag to start new Activity
            i.SetFlags(ActivityFlags.NewTask);

            // Staring Login Activity
            mContext.StartActivity(i);
        }
}
}