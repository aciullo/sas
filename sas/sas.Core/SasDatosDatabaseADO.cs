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
	public class SasDatosDatabase 
	{
		static object locker = new object ();

		public SqliteConnection connection;

		public string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        public SasDatosDatabase(string dbPath)
        {
            var output = "";
            path = dbPath;
            // create the tables
            bool exists = File.Exists(dbPath);

            if (!exists)
            {
                connection = new SqliteConnection("Data Source=" + dbPath);

                connection.Open();
                var commands = new[] {
                     "CREATE TABLE [sasDatos] (_id INTEGER PRIMARY KEY ASC, codigo TEXT, descripcion TEXT, idtabla TEXT, Activo INTEGER);"
                };
                foreach (var command in commands)
                {
                    using (var c = connection.CreateCommand())
                    {
                        c.CommandText = command;
                        var i = c.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                // already exists, do nothing. 
            }

            Console.WriteLine(output);
        }
        

		/// <summary>Convert from DataReader to Task object</summary>
		SasDatosItem FromReader (SqliteDataReader r) {
			var t = new SasDatosItem();
            
			t.ID = Convert.ToInt32 (r ["_id"]);
            t.codigo= (r["codigo"]).ToString();
            t.descripcion = (r["descripcion"]).ToString();
            t.idtabla = r ["idtabla"].ToString ();
            t.idtabla = r["idtabla"].ToString();
            t.idtabla = r["idtabla"].ToString();
            t.Activo = Convert.ToInt32(r["Activo"]) == 1 ? true : false;
            return t;
		}

		public IEnumerable<SasDatosItem> GetItems (string idtabla)
		{
			var tl = new List<SasDatosItem> ();

			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var contents = connection.CreateCommand ()) {
					contents.CommandText = "SELECT [_id], codigo, descripcion, idtabla, Activo from [sasDatos] where [idtabla] like '" + idtabla + "%'" ;
                    contents.Parameters.Add(new SqliteParameter(DbType.String) { Value = idtabla });
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

       

        public SasDatosItem GetItem (int id) 
		{
			var t = new SasDatosItem();
			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "SELECT  [_id], codigo, descripcion, idtabla, Activo from [sasDatos] WHERE _id = ?";
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

        public SasDatosItem GetItemTablaCodigo(string idtbla, string codigo)
        {
            var t = new SasDatosItem();
            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT  [_id], codigo, descripcion, idtabla, Activo from [sasDatos] WHERE idtabla like '" + idtbla + "%' and codigo like '"+ codigo +"%'";
                   // command.Parameters.Add(new SqliteParameter(DbType.String) { Value = idtbla });
                   // command.Parameters.Add(new SqliteParameter(DbType.String) { Value = codigo });


                    var r = command.ExecuteReader();
                    while (r.Read())
                    {
                        t = FromReader(r);
                        break;
                    }
                }
                connection.Close();
                GC.Collect();
            }
            return t;
        }

        public int SaveItem (SasDatosItem item) 
		{
			int r;
			lock (locker) {
				if (item.ID != 0) {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "UPDATE [sasDatos] SET [idtabla] = ? WHERE [_id] = ?;";
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.idtabla });
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
						command.CommandText = "INSERT INTO [sasDatos] (codigo, descripcion , idtabla, Activo) VALUES (?, ?, ? , ? )";
                        command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.codigo });
                        command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.descripcion });
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.idtabla });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.Activo });

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
					command.CommandText = "DELETE FROM [sasDatos] WHERE [_id] = ?;";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = id});
					r = command.ExecuteNonQuery ();
				}
				connection.Close ();
                GC.Collect();
                return r;
			}
		}


        public bool CheckIsDataAlreadyInDBorNot(string TableName,  string where)
        {
            //var t = new SasDatosItem();

            lock (locker)
            {

                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT codigo from " + TableName + " WHERE " + where;
                    var r = command.ExecuteReader();
                    if (r.HasRows)
                    {
                        return true;
                    }
                    connection.Close();
                    GC.Collect();
                    return false;

                }
            }
        }
    }
}