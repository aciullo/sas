using System;
using System.Collections.Generic;

namespace sas.Core
{
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class SasDatosManager
    {
		static SasDatosManager()
		{
		}
		
		public static SasDatosItem GetTask(int id)
		{
			return ServicioRepositoryADO.GetSasDato(id);
		}

        public static SasDatosItem GetTaskTabCod(string idtabla, string codigo)
        {
            return ServicioRepositoryADO.GetSasDatoTabCod(idtabla, codigo);
        }

        public static IList<SasDatosItem> GetTasks (string idtabla)
		{
			return new List<SasDatosItem>(ServicioRepositoryADO.GetSasDatos(idtabla));
		}

            
        public static int SaveTask (SasDatosItem item)
		{
			return ServicioRepositoryADO.SaveTaskDatos(item);
		}
		
		public static int DeleteTask(int id)
		{
			return ServicioRepositoryADO.DeleteTaskDatos(id);
		}

        public static bool CheckIsDataAlreadyInDBorNot(string TableName, string where)
        {
            return ServicioRepositoryADO.CheckIsDataAlreadyInDBorNotSasDatos(TableName, where);
        }
    }
}