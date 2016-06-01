using System;
using System.Collections.Generic;

namespace sas.Core
{
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class ServicioItemManager
    {
		static ServicioItemManager ()
		{
		}
		
		public static ServicioItem GetTask(int id)
		{
			return ServicioRepositoryADO.GetServicio(id);
		}
		
		public static IList<ServicioItem> GetTasks ()
		{
			return new List<ServicioItem>(ServicioRepositoryADO.GetServicios());
		}


        public static IList<ServicioItem> GetServiciosToSend()
        {
            return new List<ServicioItem>(ServicioRepositoryADO.GetServiciosToSend());
        }

        public static int SaveTask (ServicioItem item)
		{
			return ServicioRepositoryADO.SaveTaskDet(item);
		}
		
		public static int DeleteTask(int id)
		{
			return ServicioRepositoryADO.DeleteTaskDet(id);
		}
	}
}