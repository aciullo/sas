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
                    " codInstitucion TEXT, codDesenlace TEXT,  producto TEXT , sv_ta NTEXT, sv_fc NTEXT, sv_tempe NTEXT, "+
                    "sv_fresp NTEXT, SAT NTEXT, Glasgow NTEXT, Glicemia NTEXT, IndicacionArribo NTEXT );",



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
        ServicioLocalItem FromReader(SqliteDataReader r)
        {
            var t = new ServicioLocalItem();
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
            t.sv_ta = r["sv_ta"].ToString();
            t.sv_fc = r["sv_fc"].ToString();
            t.sv_tempe = r["sv_tempe"].ToString();
            t.sv_fresp = r["sv_fresp"].ToString();
            t.SAT = r["SAT"].ToString();
            t.Glasgow = r["Glasgow"].ToString();
            t.Glicemia = r["Glicemia"].ToString();
            t.IndicacionArribo = r["IndicacionArribo"].ToString();
            //Convert.ToInt32 (r ["Done"]) == 1 ? true : false;
            return t;

        }

        public IEnumerable<ServicioLocalItem> GetItems(string idmovil)
        {
            var tl = new List<ServicioLocalItem>();

            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var contents = connection.CreateCommand())
                { try { 
                    contents.CommandText = "SELECT [_id], [id_solicitud], [NumeroSolicitud], [Fecha_Llamado], [hora_Llamado]," +
                                            "[nombrePaciente], [Tel], [edadPaciente], [nombreSolicitante], [direccionReferecia], " +
                                            "[direccionReferecia2], [numeroCasa], [referencia], [Motivo], [nroSalida], [codMovil]," +
                                            "[codChofer], [Acompañante],[observacion], [Estado], [codEstado], [HoraEstado], "+
                                            "[codMotivo1], [codMotivo2], [codMotivo3], [OtroMotivo], [codTipo], [codInstitucion],"+
                                            " [codDesenlace], [producto], sv_ta , sv_fc , sv_tempe , sv_fresp , SAT , Glasgow , "+
                                            "Glicemia ,IndicacionArribo  " +
                                            "from [ServicioCab] WHERE [codEstado] <> '009' and [codMovil]=?";
                        contents.Parameters.Add(new SqliteParameter(DbType.String) { Value = idmovil });
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

        public ServicioLocalItem GetItem(int id)
        {
            var t = new ServicioLocalItem();
            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [_id], [id_solicitud], [NumeroSolicitud], [Fecha_Llamado], "+
                                          "[hora_Llamado],[nombrePaciente], [Tel], [edadPaciente], [nombreSolicitante], "+
                                          "[direccionReferecia], [direccionReferecia2], [numeroCasa], [referencia], "+
                                          "[Motivo], [nroSalida], [codMovil],[codChofer], [Acompañante],[observacion], "+
                                          "[Estado], [codEstado], [HoraEstado], [codMotivo1], [codMotivo2], [codMotivo3], "+
                                          "[OtroMotivo], [codTipo], [codInstitucion], [codDesenlace], [producto] "+
                                          ",sv_ta , sv_fc , sv_tempe , sv_fresp , SAT , Glasgow , Glicemia, IndicacionArribo " +
                                          " from [ServicioCab] WHERE [_id] = ?";
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
        public ServicioLocalItem GetTaskIdSol(int id)
        {
            var t = new ServicioLocalItem();
            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [_id], [id_solicitud], [NumeroSolicitud], [Fecha_Llamado], " +
                                          "[hora_Llamado],[nombrePaciente], [Tel], [edadPaciente], [nombreSolicitante], " +
                                          "[direccionReferecia], [direccionReferecia2], [numeroCasa], [referencia], " +
                                          "[Motivo], [nroSalida], [codMovil],[codChofer], [Acompañante],[observacion], " +
                                          "[Estado], [codEstado], [HoraEstado], [codMotivo1], [codMotivo2], [codMotivo3], " +
                                          "[OtroMotivo], [codTipo], [codInstitucion], [codDesenlace], [producto] " +
                                          ",sv_ta , sv_fc , sv_tempe , sv_fresp , SAT , Glasgow , Glicemia, IndicacionArribo " +
                                          " from [ServicioCab] WHERE [id_solicitud] = ?";
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
        public int SaveItem(ServicioLocalItem item)
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
                        command.CommandText = "UPDATE [ServicioCab] SET [Estado] = ?, [codEstado] = ?, [HoraEstado]= ? , "+
                                                "[codInstitucion] = ? , [codDesenlace] = ?, [sv_ta]= ? , [sv_fc] = ?, [sv_tempe]= ? , "+
                                                " [sv_fresp]= ?  , [SAT]= ? , [Glasgow]= ? , [Glicemia]= ?, [IndicacionArribo] = ? "+
                                                " WHERE [_id] = ?;";
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Estado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codEstado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.HoraEstado });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codInstitucion });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.codDesenlace });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.sv_ta });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.sv_fc });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.sv_tempe });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.sv_fresp });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.SAT });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Glasgow });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Glicemia });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.IndicacionArribo });
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

        public int DeleteAllSendedItem()
        {
            lock (locker)
            {
                int r;
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM [ServicioCab];";
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
            var t = new ServicioLocalItem();

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
            var t = new ServicioLocalItem();

            lock (locker)
            {

                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(id_solicitud) cantidad from [ServicioCab] WHERE codestado <>'002' and codestado <> '009' ";
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