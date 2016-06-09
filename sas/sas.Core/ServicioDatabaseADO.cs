using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using Android.OS;

namespace sas.Core
{
    /// <summary>
    /// TaskDatabase uses ADO.NET to create the [Items] table and create,read,update,delete data
    /// </summary>
    public class ServicioDatabase
    {
        static object locker = new object();

        public SqliteConnection connection;

        public string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        public ServicioDatabase(string dbPath)
        {

            var output = "";
            path = dbPath;
            // create the tables
            bool exists = File.Exists(dbPath);

            if (!exists)
            {
                connection = new SqliteConnection("Data Source=" + dbPath);

                connection.Open();
                try
                {
                    var commands = new[] {
                    "CREATE TABLE [ServicioCab] (_id INTEGER PRIMARY KEY ASC, id_solicitud INTEGER, " + 
                    " NumeroSolicitud INTEGER, fecha_Llamado TEXT ,  hora_Llamado TEXT, nombrePaciente TEXT, " +
                    " Tel TEXT, edadPaciente NUMERIC, nombreSolicitante TEXT, direccionReferecia TEXT, " +
                    " direccionReferecia2 TEXT, numeroCasa TEXT, referencia TEXT, Motivo TEXT,  nroSalida TEXT, " +
                    " codMovil TEXT, codChofer TEXT, Acompañante TEXT, observacion TEXT, Estado TEXT,  codEstado TEXT, " +
                    " HoraEstado TEXT,codMotivo1 TEXT, codMotivo2 TEXT, codMotivo3 TEXT,  OtroMotivo TEXT  , codTipo TEXT, " +
                    " codInstitucion TEXT, codDesenlace TEXT,  producto TEXT );",

                    "CREATE TABLE [ServiciosDet] (_id INTEGER PRIMARY KEY ASC, id_solicitud INTEGER, " +
                    " NumeroSolicitud INTEGER, Nombre NTEXT, Fecha NTEXT, codMovil NTEXT, Estado NTEXT, " +
                    " codEstado NTEXT, HoraEstado NTEXT, codInstitucion NTEXT, codDesenlace NTEXT, Enviado INTEGER, " +
                    "AuditUsuario NTEXT, AuditId INTEGER,GeoData NTEXT, Address NTEXT );" ,

                     "CREATE TABLE [sasDatos] (_id INTEGER PRIMARY KEY ASC, codigo TEXT, descripcion TEXT, idtabla TEXT);"
                };
                    foreach (var command in commands)
                    {
                        using (var c = connection.CreateCommand())
                        {
                            c.CommandText = command;
                            var i = c.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                    GC.Collect();
                }
                catch (Exception ex) { }
            }
            else
            {
                // already exists, do nothing. 
            }
            Console.WriteLine(output);
        }

        /// <summary>Convert from DataReader to Task object</summary>
        ServicioLocal FromReader(SqliteDataReader r)
        {
            var t = new ServicioLocal();
            t.ID = Convert.ToInt32(r["_id"]);
            t.id_Solicitud = Convert.ToInt32(r["id_solicitud"]);
            t.NumeroSolicitud = Convert.ToInt32(r["NumeroSolicitud"]);
            t.fecha_Llamado = Convert.ToDateTime(r["fecha_Llamado"]);
            t.hora_Llamado = r["hora_Llamado"].ToString();
            t.nombrePaciente = r["nombrePaciente"].ToString();
            t.Tel = r["Tel"].ToString();
            t.edadPaciente = Convert.ToDecimal(r["edadPaciente"]);
            t.nombreSolicitante = r["nombreSolicitante"].ToString();
            t.direccionReferecia = r["direccionReferecia"].ToString();
            t.direccionReferecia2 = r["direccionReferecia2"].ToString();
            t.numeroCasa = r["numeroCasa"].ToString();
            t.referencia = r["referencia"].ToString();
            t.Motivo = r["Motivo"].ToString();
            t.nroSalida = r["nroSalida"].ToString();
            t.codMovil = r["codMovil"].ToString();
            t.codChofer = r["codChofer"].ToString();
            t.Acompañante = r["Acompañante"].ToString();
            t.observacion = r["observacion"].ToString();
            t.Estado = r["Estado"].ToString();
            t.codEstado = r["codEstado"].ToString();
            t.HoraEstado = r["HoraEstado"].ToString();
            t.codMotivo1 = r["codMotivo1"].ToString();
            t.codMotivo2 = r["codMotivo2"].ToString();
            t.codMotivo3 = r["codMotivo3"].ToString();
            t.OtroMotivo = r["OtroMotivo"].ToString();
            t.codTipo = r["codTipo"].ToString();
            t.codInstitucion = r["codInstitucion"].ToString();
            t.codDesenlace = r["codDesenlace"].ToString();
            t.producto = r["producto"].ToString();

            //Convert.ToInt32 (r ["Done"]) == 1 ? true : false;
            return t;

        }

        public IEnumerable<ServicioLocal> GetItems()
        {
            var tl = new List<ServicioLocal>();

            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var contents = connection.CreateCommand())
                { try { 
                    contents.CommandText = "SELECT [_id], [id_solicitud], [NumeroSolicitud], [Fecha_Llamado], [hora_Llamado],[nombrePaciente], [Tel], [edadPaciente], [nombreSolicitante], [direccionReferecia], [direccionReferecia2], [numeroCasa], [referencia], [Motivo], [nroSalida], [codMovil],[codChofer], [Acompañante],[observacion], [Estado], [codEstado], [HoraEstado], [codMotivo1], [codMotivo2], [codMotivo3], [OtroMotivo], [codTipo], [codInstitucion], [codDesenlace], [producto]  from [ServicioCab] WHERE [codEstado] <> '009' ";
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        tl.Add(FromReader(r));
                    }

                    } catch (Exception ex)

                    {

                    }
                }
                connection.Close();
                GC.Collect();
            }
            return tl;
        }

        public ServicioLocal GetItem(int id)
        {
            var t = new ServicioLocal();
            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [_id], [id_solicitud], [NumeroSolicitud], [Fecha_Llamado], [hora_Llamado],[nombrePaciente], [Tel], [edadPaciente], [nombreSolicitante], [direccionReferecia], [direccionReferecia2], [numeroCasa], [referencia], [Motivo], [nroSalida], [codMovil],[codChofer], [Acompañante],[observacion], [Estado], [codEstado], [HoraEstado], [codMotivo1], [codMotivo2], [codMotivo3], [OtroMotivo], [codTipo], [codInstitucion], [codDesenlace], [producto]  from [ServicioCab] WHERE [_id] = ?";
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id });
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

        public int SaveItem(ServicioLocal item)
        {
            int r;
            lock (locker)
            {
                if (item.ID != 0)
                {
                    connection = new SqliteConnection("Data Source=" + path);
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE [ServicioCab] SET [Estado] = ?, [codEstado] = ?, [HoraEstado]= ? , [codInstitucion] = ? , [codDesenlace] = ? WHERE [_id] = ?;";
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Estado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codEstado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.HoraEstado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codInstitucion });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codDesenlace });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.ID });

                        r = command.ExecuteNonQuery();
                    }
                    connection.Close();
                    GC.Collect();
                    return r;
                }
                else
                {
                    connection = new SqliteConnection("Data Source=" + path);
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO [ServicioCab] ( id_solicitud  , NumeroSolicitud  , fecha_Llamado ,  hora_Llamado " +
                                               " , nombrePaciente  , Tel  , edadPaciente , nombreSolicitante , " +
                                               "  direccionReferecia  , direccionReferecia2  , numeroCasa  , referencia  , Motivo  , " +
                                               " nroSalida  , codMovil  , codChofer  , Acompañante  , observacion  , Estado  , " +
                                               " codEstado  , HoraEstado  ,codMotivo1  , codMotivo2  , codMotivo3  ,  OtroMotivo   " +
                                               " , codTipo  , codInstitucion  , codDesenlace  ,  producto   ) VALUES (? ,?, ?,? ,?, ?,? ,?, ?,? ,?, ?,? ,?, ?,? ,? , ? ,?, ?,? ,?, ?,? ,?, ?,? ,?, ?)";
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.id_Solicitud });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.NumeroSolicitud });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.fecha_Llamado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.hora_Llamado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.nombrePaciente });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Tel });
                        command.Parameters.Add(new SqliteParameter(DbType.VarNumeric) { Value = item.edadPaciente });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.nombreSolicitante });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.direccionReferecia });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.direccionReferecia2 });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.numeroCasa });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.referencia });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Motivo });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.nroSalida });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codMovil });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codChofer });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Acompañante });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.observacion });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Estado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codEstado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.HoraEstado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codMotivo1 });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codMotivo2 });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codMotivo3 });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.OtroMotivo });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codTipo });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codInstitucion });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codDesenlace });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.producto });



                        r = command.ExecuteNonQuery();
                    }
                    connection.Close();
                    GC.Collect();
                    return r;
                }

            }
        }

        public int DeleteItem(int id)
        {
            lock (locker)
            {
                int r;
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM [ServicioCab] WHERE [_id] = ?;";
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id });
                    r = command.ExecuteNonQuery();
                }
                connection.Close();
                GC.Collect();
                return r;
            }
        }

        public bool CheckIsDataAlreadyInDBorNot(string TableName,
        string dbfield, string fieldValue)
        {
            var t = new ServicioLocal();

            lock (locker)
            {

                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id_solicitud from " + TableName + " WHERE " + dbfield + " = " + fieldValue;
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

        public int CantidadPendiente()
        {
            var t = new ServicioLocal();

            lock (locker)
            {

                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(id_solicitud) cantidad from [ServicioCab] WHERE codestado <>'001' and codestado <> '009' ";
                    var r = command.ExecuteReader();
                    if (r.HasRows)
                    {
                        return Convert.ToInt32(r["cantidad"]);
                    }
                    connection.Close();
                    GC.Collect();
                    return 0;

                }
            }
        }
    }
}