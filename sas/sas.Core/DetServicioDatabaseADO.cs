using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using Android.Content;

namespace sas.Core
{
	/// <summary>
	/// TaskDatabase uses ADO.NET to create the [Items] table and create,read,update,delete data
	/// </summary>
	public class DetServicioDatabase 
	{
		static object locker = new object ();

		public SqliteConnection connection;

		public string path;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		public DetServicioDatabase(string dbPath) 
		{
			var output = "";
			path = dbPath;
			// create the tables
			bool exists = File.Exists (dbPath);

			if (!exists) {
				connection = new SqliteConnection ("Data Source=" + dbPath);

				connection.Open ();
				var commands = new[] {
                       "CREATE TABLE [ServiciosDet] (_id INTEGER PRIMARY KEY ASC, id_solicitud INTEGER, " +
                    " NumeroSolicitud INTEGER, Nombre NTEXT, Fecha NTEXT, codMovil NTEXT, Estado NTEXT, " +
                    " codEstado NTEXT, HoraEstado NTEXT, codInstitucion NTEXT, codDesenlace NTEXT, Enviado INTEGER, " +
                    "AuditUsuario NTEXT, AuditId INTEGER,GeoData NTEXT, Address NTEXT, sv_ta NTEXT, sv_fc NTEXT, sv_tempe NTEXT, sv_fresp NTEXT, SAT NTEXT, Glasgow NTEXT, Glicemia NTEXT );" ,
                };
				foreach (var command in commands) {
					using (var c = connection.CreateCommand ()) {
						c.CommandText = command;
						var i = c.ExecuteNonQuery ();
					}
				}
			} else {
				// already exists, do nothing. 
			}
			Console.WriteLine (output);
		}

		/// <summary>Convert from DataReader to Task object</summary>
		ServicioItem FromReader (SqliteDataReader r) {
			var t = new ServicioItem ();
            
			t.ID = Convert.ToInt32 (r ["_id"]);
            t.id_Solicitud= Convert.ToInt32(r["id_solicitud"]);
            t.NumeroSolicitud = Convert.ToInt32(r["NumeroSolicitud"]);
            t.Nombre = r ["Nombre"].ToString ();
            t.Fecha = Convert.ToDateTime( r ["Fecha"]);
            t.codMovil = r ["codMovil"].ToString ();
            t.Estado = r ["Estado"].ToString ();
            t.codEstado = r ["codEstado"].ToString ();
            t.HoraEstado = r ["HoraEstado"].ToString ();
            t.codInstitucion = r ["codInstitucion"].ToString ();
            t.codDesenlace = r ["codDesenlace"].ToString ();
            t.Enviado = Convert.ToInt32 (r ["Enviado"]) == 1 ? true : false;
            t.AuditUsuario = r["AuditUsuario"].ToString();
            t.AuditId = Convert.ToInt32(r["AuditId"]);
            t.GeoData = r["GeoData"].ToString();
            t.Address= r["Address"].ToString();
            t.sv_ta = r["sv_ta"].ToString();
            t.sv_fc = r["sv_fc"].ToString();
            t.sv_tempe= r["sv_tempe"].ToString();
            t.sv_fresp = r["sv_fresp"].ToString();
            t.SAT = r["SAT"].ToString();
            t.Glasgow = r["Glasgow"].ToString();
            t.Glicemia = r["Glicemia"].ToString();

            return t;
		}

		public IEnumerable<ServicioItem> GetItems ()
		{
			var tl = new List<ServicioItem> ();

			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var contents = connection.CreateCommand ()) {
					contents.CommandText = "SELECT [_id], id_solicitud, NumeroSolicitud, Nombre, Fecha, codMovil, Estado, " + 
                                           " codEstado, HoraEstado, codInstitucion, codDesenlace, [Enviado], AuditUsuario , " +
                                           " AuditId ,GeoData,Address, sv_ta , sv_fc , sv_tempe , sv_fresp , SAT , Glasgow , Glicemia   from [ServiciosDet]";
					var r = contents.ExecuteReader ();
					while (r.Read ()) {
						tl.Add (FromReader(r));
					}
				}
				connection.Close ();
                GC.Collect();
            }
			return tl;
		}

        public IEnumerable<ServicioItem> GetItemsToSend()
        {
            var tl = new List<ServicioItem>();

            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var contents = connection.CreateCommand())
                {
                    contents.CommandText = "SELECT [_id], id_solicitud, NumeroSolicitud, Nombre, Fecha, codMovil, Estado, " +
                                           " codEstado, HoraEstado, codInstitucion, codDesenlace, [Enviado] , AuditUsuario , " +
                                           " AuditId ,GeoData,Address, sv_ta , sv_fc , sv_tempe , sv_fresp , SAT , Glasgow , Glicemia " +
                                           " from [ServiciosDet] WHERE [Enviado] = 0  ";
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        tl.Add(FromReader(r));
                    }
                }
                connection.Close();
                GC.Collect();
            }
            return tl;
        }

        public ServicioItem GetItem (int id) 
		{
			var t = new ServicioItem();
			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "SELECT  [_id], id_solicitud, NumeroSolicitud, Nombre, Fecha, codMovil, Estado, " +
                                          " codEstado, HoraEstado, codInstitucion, codDesenlace, [Enviado], AuditUsuario , " +
                                          " AuditId ,GeoData,Address, sv_ta , sv_fc , sv_tempe , sv_fresp , SAT , Glasgow , Glicemia  from [ServiciosDet] WHERE [_id] = ?";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = id });
					var r = command.ExecuteReader ();
					while (r.Read ()) {
						t = FromReader (r);
						break;
					}
				}
				connection.Close ();
                GC.Collect();
            }
			return t;
		}



        public IEnumerable<ServicioItem> GetItemByForeingID(int id)
        {
            var tl = new List<ServicioItem>();

            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var contents = connection.CreateCommand())
                {
                    contents.CommandText = "SELECT  [_id], id_solicitud, NumeroSolicitud, Nombre, Fecha, codMovil, Estado, " +
                                           " codEstado, HoraEstado, codInstitucion, codDesenlace, [Enviado], AuditUsuario , " +
                                           " AuditId ,GeoData,Address , sv_ta , sv_fc , sv_tempe , sv_fresp , SAT , Glasgow , Glicemia from [ServiciosDet] WHERE [AuditId] = ?";
                    contents.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id });
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        tl.Add(FromReader(r));
                    }
                }
                connection.Close();
                GC.Collect();
            }
            return tl;
        }


        public int SaveItem (ServicioItem item) 
		{
			int r;
			lock (locker) {
				if (item.ID != 0) {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "UPDATE [ServiciosDet] SET [Enviado] = ? WHERE [_id] = ?;";
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.Enviado });
						//command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.Notes });
						//command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.Done });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.ID });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
                    GC.Collect();
                    return r;
				} else {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "INSERT INTO [ServiciosDet] (id_solicitud, NumeroSolicitud, Nombre, Fecha, codMovil, " + 
                                              " Estado, codEstado, HoraEstado, codInstitucion, codDesenlace, [Enviado], " +
                                              " AuditUsuario , AuditId ,GeoData,Address) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ? , ?)";
                        //command.CommandText = "INSERT INTO [Servicios] (_id], [Nombre], [NroServicio], [Enviado]) VALUES (? ,?, ?, ?)";
                        command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.id_Solicitud });
                        command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.NumeroSolicitud });
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.Nombre });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Fecha });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codMovil });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Estado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codEstado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.HoraEstado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codInstitucion });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codDesenlace });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.Enviado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.AuditUsuario });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.AuditId });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.GeoData });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Address });

                        r = command.ExecuteNonQuery ();
					}
					connection.Close ();
                    GC.Collect();
                    return r;
				}

			}
		}

		public int DeleteItem(int id) 
		{
			lock (locker) {
				int r;
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "DELETE FROM [ServiciosDet] WHERE [_id] = ?;";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = id});
					r = command.ExecuteNonQuery ();
				}
				connection.Close ();
                GC.Collect();
                return r;
			}
		}
	}
}