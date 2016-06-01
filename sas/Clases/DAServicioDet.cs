using System;
using SQLite.Net;
using System.Collections.Generic;
using System.Linq;
using SQLite.Net.Interop;

namespace sas
{
	public class DAServicioDet: IDisposable
	{
		private SQLiteConnection connection;
        private string directorioDB;
        private ISQLitePlatform plataforma;
        public DAServicioDet()
		{

            
           // var config = 
			connection = new SQLiteConnection(Plataforma,
				System.IO.Path.Combine(DirectorioDB, "Servicios.db3"));
			connection.CreateTable<RegistrarServicioModel>();
		}

        public ISQLitePlatform Plataforma
        {
            get
            {
                if (plataforma == null)
                {

                    plataforma = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();

                }

                return plataforma;
            }
        }

        public string DirectorioDB
        {
            get
            {
                if (string.IsNullOrEmpty(directorioDB))
                {
                    directorioDB = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }

                return directorioDB;
            }
        }
        public void InsertServicio(RegistrarServicioModel servicio)
		{
			connection.Insert(servicio);
		}

		public void UpdateServicio(RegistrarServicioModel servicio)
		{
			connection.Update(servicio);
		}

		public void DeleteServicio(RegistrarServicioModel servicio)
		{
			connection.Delete(servicio);
		}

		public RegistrarServicioModel GetServicio(int IDregistro)
		{
			return connection.Table<RegistrarServicioModel>().FirstOrDefault(c => c.id_Solicitud == IDregistro);
		}

		public List<RegistrarServicioModel> GetServicios()
		{
			return connection.Table<RegistrarServicioModel>().OrderBy(c => c.id_Solicitud).ToList();
		}

		public void Dispose()
		{
			connection.Dispose();
		}
	}
}

