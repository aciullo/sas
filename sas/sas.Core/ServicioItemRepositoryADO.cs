using System;
using System.Collections.Generic;
using System.IO;

namespace sas.Core {

    public class ServicioItemRepositoryADO {
		ServicioDatabase db = null;
        DetServicioDatabase dbdet = null;
        SasDatosDatabase dbdatos = null;
        protected static string dbLocation;		
		protected static ServicioItemRepositoryADO me;		

		static ServicioItemRepositoryADO()
		{
			me = new ServicioItemRepositoryADO();

		}

		protected ServicioItemRepositoryADO()
		{
			// set the db location
			dbLocation = DatabaseFilePath;

            // instantiate the database	
            dbdet = new DetServicioDatabase(dbLocation);

        

        }

        public static string DatabaseFilePath {
			get { 
				var sqliteFilename = "ServicioItemDatabase.db3";
				#if NETFX_CORE
				var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
				#else

				#if SILVERLIGHT
				// Windows Phone expects a local path, not absolute
				var path = sqliteFilename;
				#else

				#if __ANDROID__
				// Just use whatever directory SpecialFolder.Personal returns
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
				#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
				#endif
				var path = Path.Combine (libraryPath, sqliteFilename);
				#endif

				#endif
				return path;	
			}
		}


        // servios det 
        public static ServicioItem GetServicio(int id)
        {
            return me.dbdet.GetItem(id);
        }

        public static IEnumerable<ServicioItem> GetItemByForeingID(int id)
        {
            return me.dbdet.GetItemByForeingID(id);
        }
        public static IEnumerable<ServicioItem> GetServicios()
        {
            return me.dbdet.GetItems();
        }

        public static IEnumerable<ServicioItem> GetServiciosToSend()
        {
            return me.dbdet.GetItemsToSend();
        }

        public static int SaveTaskDet(ServicioItem item)
        {
            return me.dbdet.SaveItem(item);
        }

        public static int DeleteTaskDet(int id)
        {
            return me.dbdet.DeleteItem(id);
        }

    
    }
}

