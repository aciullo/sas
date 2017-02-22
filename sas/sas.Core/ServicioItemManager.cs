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
			return ServicioItemRepositoryADO.GetServicio(id);
		}

        public static IList<ServicioItem> GetItemByForeingID (int id)
        {
            return new List<ServicioItem> (ServicioItemRepositoryADO.GetItemByForeingID(id));
        }

        public static IList<ServicioItem> GetTasks ()
		{
			return new List<ServicioItem>(ServicioItemRepositoryADO.GetServicios());
		}


        public static IList<ServicioItem> GetServiciosToSend()
        {
            return new List<ServicioItem>(ServicioItemRepositoryADO.GetServiciosToSend());
        }

        public static IList<ServicioItem> GetServiciosSended()
        {
            return new List<ServicioItem>(ServicioItemRepositoryADO.GetServiciosSended());
        }

        public static int SaveTask (ServicioItem item)
		{
			return ServicioItemRepositoryADO.SaveTaskDet(item);
		}
		
		public static int DeleteTask(int id)
		{
			return ServicioItemRepositoryADO.DeleteTaskDet(id);
		}
	}
}