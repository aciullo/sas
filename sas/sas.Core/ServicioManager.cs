using System;
using System.Collections.Generic;

namespace sas.Core {
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class ServicioManager {

		static ServicioManager()
		{
		}
		
		public static ServicioLocal GetTask(int id)
		{
			return ServicioRepositoryADO.GetTask(id);
		}
		
		public static IList<ServicioLocal> GetTasks ()
		{
			return new List<ServicioLocal>(ServicioRepositoryADO.GetTasks());
		}
		
		public static int SaveTask (ServicioLocal item)
		{
			return ServicioRepositoryADO.SaveTask(item);
		}
		
		public static int DeleteTask(int id)
		{
			return ServicioRepositoryADO.DeleteTask(id);
		}

        public static bool CheckIsDataAlreadyInDBorNot(string TableName, string dbfield, string fieldValue)
        {
            
            return ServicioRepositoryADO.CheckIsDataAlreadyInDBorNot(TableName, dbfield, fieldValue);
        }

        public static int CantidadPendiente()
        {

            return ServicioRepositoryADO.CantidadPendiente();
        }
    }
}