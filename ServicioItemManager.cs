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
		
		public static IList<ServicioItem> GetTasks ()
		{
			return new List<ServicioItem>(ServicioItemRepositoryADO.GetServicios());
		}


        public static IList<ServicioItem> GetServiciosToSend()
        {
            return new List<ServicioItem>(ServicioItemRepositoryADO.GetServiciosToSend());
        }

        public static int SaveTask (ServicioItem item)
		{
			return ServicioItemRepositoryADO.SaveTask(item);
		}
		
		public static int DeleteTask(int id)
		{
			return ServicioItemRepositoryADO.DeleteTask(id);
		}
	}
}