using System;
using System.Collections.Generic;
using System.IO;

namespace sas.Core {

    public class ServicioRepositoryADO {
		ServicioDatabase db = null;
        DetServicioDatabase dbdet = null;
        SasDatosDatabase dbdatos = null;
        protected static string dbLocation;		
		protected static ServicioRepositoryADO me;		

		static ServicioRepositoryADO()
		{
			me = new ServicioRepositoryADO();

		}

		protected ServicioRepositoryADO()
		{
			// set the db location
			dbLocation = DatabaseFilePath;

			// instantiate the database	
			db = new ServicioDatabase(dbLocation);

            //dbdet = new DetServicioDatabase(dbLocation);

            dbdatos = new SasDatosDatabase(dbLocation);

        }

        public static string DatabaseFilePath {
			get { 
				var sqliteFilename = "SasDatabase.db3";
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

		public static ServicioLocalItem GetTask(int id)
		{
			return me.db.GetItem(id);
		}
        public static ServicioLocalItem GetTaskIdSol(int id)
        {
            return me.db.GetTaskIdSol(id);
        }
        public static IEnumerable<ServicioLocalItem> GetTasks (string idmovil)
		{
			return me.db.GetItems(idmovil);
		}

		public static int SaveTask (ServicioLocalItem item)
		{
			return me.db.SaveItem(item);
		}

		public static int DeleteTask(int id)
		{
			return me.db.DeleteItem(id);
		}
        public static bool CheckIsDataAlreadyInDBorNot(string TableName, string dbfield, string fieldValue)
        {
            return me.db.CheckIsDataAlreadyInDBorNot(TableName, dbfield, fieldValue);
        }

        public static int  CantidadPendiente()
        {
            return me.db.CantidadPendiente();
        }

        // servicios det 
        //public static ServicioItem GetServicio(int id)
        //{
        //    return me.dbdet.GetItem(id);
        //}

        //public static IEnumerable<ServicioItem> GetItemByForeingID(int id)
        //{
        //    return me.dbdet.GetItemByForeingID(id);
        //}
        //public static IEnumerable<ServicioItem> GetServicios()
        //{
        //    return me.dbdet.GetItems();
        //}

        //public static IEnumerable<ServicioItem> GetServiciosToSend()
        //{
        //    return me.dbdet.GetItemsToSend();
        //}

        //public static int SaveTaskDet(ServicioItem item)
        //{
        //    return me.dbdet.SaveItem(item);
        //}

        //public static int DeleteTaskDet(int id)
        //{
        //    return me.dbdet.DeleteItem(id);
        //}

        //sas datos
        public static int SaveTaskDatos(SasDatosItem item)
        {
            return me.dbdatos.SaveItem(item);
        }

        public static int DeleteTaskDatos(int id)
        {
            return me.dbdatos.DeleteItem(id);
        }

        public static SasDatosItem GetSasDato(int id)
        {
            return me.dbdatos.GetItem(id);
        }
        public static SasDatosItem GetSasDatoTabCod(string idtabla, string codigo)
        {
            return me.dbdatos.GetItemTablaCodigo(idtabla, codigo);
        }
        public static IEnumerable<SasDatosItem> GetSasDatos(string idtabla)
        {
            return me.dbdatos.GetItems(idtabla);
        }

        public static bool CheckIsDataAlreadyInDBorNotSasDatos(string TableName, string where)
        {
            return me.dbdatos.CheckIsDataAlreadyInDBorNot(TableName, where);
        }
    }
}

