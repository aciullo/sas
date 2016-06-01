using System;
using SQLite.Net;
using System.Collections.Generic;
using System.Linq;
using SQLite.Net.Interop;
using sas.Models;


namespace sas
{
	public class DAServicioCab: IDisposable
	{
		private SQLiteConnection connection;
        private string directorioDB;
        private ISQLitePlatform plataforma;
        public DAServicioCab()
		{

            try {

                // var config = 
                connection = new SQLiteConnection(Plataforma,
                             System.IO.Path.Combine(DirectorioDB, "Servicios.db3"));
                connection.CreateTable<ServiciosLocalModel>();

            } catch (Exception ex)
            {
                
            }
           
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
        public void InsertServicio(ServiciosLocalModel servicio)
		{
			connection.Insert(servicio);
		}

		public void UpdateServicio(ServiciosLocalModel servicio)
		{
			connection.Update(servicio);
		}

		public void DeleteServicio(ServiciosLocalModel servicio)
		{
			connection.Delete(servicio);
		}

		public ServiciosLocalModel GetServicio(int IDregistro)
		{
			return connection.Table<ServiciosLocalModel>().FirstOrDefault(c => c.id_Solicitud == IDregistro);
		}

		public List<ServiciosLocalModel> GetServicios()
		{
			return connection.Table<ServiciosLocalModel>().OrderBy(c => c.id_Solicitud).ToList();
		}

		public void Dispose()
		{
			connection.Dispose();
		}
	}
}

