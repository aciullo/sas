using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using Android.Content;

namespace Sas.Shared
{
	/// <summary>
	/// TaskDatabase uses ADO.NET to create the [Items] table and create,read,update,delete data
	/// </summary>
	public class SasDatabase 
	{
		static object locker = new object ();

		public SqliteConnection connection;

		public string path;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		public SasDatabase(string dbPath) 
		{
			var output = "";
			path = dbPath;
			// create the tables
			bool exists = File.Exists (dbPath);

			if (!exists) {
				connection = new SqliteConnection ("Data Source=" + dbPath);

				connection.Open ();
				var commands = new[] {
                    "CREATE TABLE [Servicios] (_id INTEGER PRIMARY KEY ASC, Nombre NTEXT, NroServicio INTEGER, Enviado INTEGER);"
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
            
			t.IdServicio = Convert.ToInt32 (r ["_id"]);
			t.Nombre = r ["Nombre"].ToString ();
			t.NroServicio = Convert.ToInt32 (r ["NroServicio"]);
			t.Enviado = Convert.ToInt32 (r ["Enviado"]) == 1 ? true : false;
			return t;
		}

		public IEnumerable<ServicioItem> GetItems ()
		{
			var tl = new List<ServicioItem> ();

			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var contents = connection.CreateCommand ()) {
					contents.CommandText = "SELECT [_id], [Nombre], [NroServicio], [Enviado] from [Servicios]";
					var r = contents.ExecuteReader ();
					while (r.Read ()) {
						tl.Add (FromReader(r));
					}
				}
				connection.Close ();
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
                    contents.CommandText = "SELECT [_id], [Nombre], [NroServicio], [Enviado] from [Servicios] where Enviado = 0";
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        tl.Add(FromReader(r));
                    }
                }
                connection.Close();
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
					command.CommandText = "SELECT [_id], [Nombre], [NroServicio], [Enviado] from [Servicios] WHERE [_id] = ?";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = id });
					var r = command.ExecuteReader ();
					while (r.Read ()) {
						t = FromReader (r);
						break;
					}
				}
				connection.Close ();
			}
			return t;
		}

		public int SaveItem (ServicioItem item) 
		{
			int r;
			lock (locker) {
				if (item.IdServicio != 0) {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "UPDATE [Servicios] SET [Enviado] = ? WHERE [_id] = ?;";
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.Enviado });
						//command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.Notes });
						//command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.Done });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.IdServicio });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
                 
                    return r;
				} else {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "INSERT INTO [Servicios] ([Nombre], [NroServicio], [Enviado]) VALUES (?, ?, ?)";
                        //command.CommandText = "INSERT INTO [Servicios] (_id], [Nombre], [NroServicio], [Enviado]) VALUES (? ,?, ?, ?)";
                        //command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.IdServicio });
                        command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.Nombre });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.NroServicio });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.Enviado });
                        r = command.ExecuteNonQuery ();
					}
					connection.Close ();
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
					command.CommandText = "DELETE FROM [Servicios] WHERE [_id] = ?;";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = id});
					r = command.ExecuteNonQuery ();
				}
				connection.Close ();
				return r;
			}
		}
	}
}