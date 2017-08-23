using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using sas_Futura.Controllers.Services;
using sas_Futura.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
namespace sas_Futura.Controllers.Services
{
    public class JsonRepository
    {
        DataAccess acc = new DataAccess();
        public string GetAllJsons(int id_Solicitud, string codEstado, string hora)
        {

            SqlConnection conn = null;
            SqlCommand command = null;
            SqlTransaction mytrans = null;
            var connectionString = string.Empty;
            var json = string.Empty;
            connectionString = acc.leerConeccion();


            try
            {

                command = new SqlCommand();

                conn = new SqlConnection(connectionString);
                conn.Open();
                //mytrans = conn.BeginTransaction(IsolationLevel.ReadCommitted, "ActualizarEstado");
                //command.Connection = conn;
                //command.Transaction = mytrans;

                //command.Parameters.Clear();
                command = new SqlCommand("Sas_Update_Servicio", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@IdServicio", id_Solicitud));
                command.Parameters.Add(new SqlParameter("@IdEstado", codEstado));
                command.Parameters.Add(new SqlParameter("@Hora", hora));


                command.ExecuteNonQuery();

                //mytrans.Commit();

                return "OK";
            }
            catch (Exception ex)
            {
                
                return "Error";
            }
            finally
            {
                conn.Close();
            }

        }
        public List<sasDatosModel> GetAllSasDatos(string idtabla, string codigo)
        {
            SqlDataReader rdr = null;
            SqlConnection conn = null;
            SqlCommand command = null;
            var connectionString = string.Empty;
            var json = string.Empty;
            var datos = new sasDatosModel();


            var searchResults = new List<sasDatosModel>();

            connectionString = acc.leerConeccion();
            string sql = "";



            conn = new SqlConnection(connectionString);

            command = new SqlCommand();

            command.Connection = conn;

            command.CommandType = CommandType.Text;

            command.CommandTimeout = 1200;

            if (codigo == null)
            {
                sql = "SELECT codigo, nombre, idtabla from sas_datos where  idtabla = '" + idtabla + "' and idempresa= '" + Startup.IdEmpresa + "'  order by nombre ";
            }
            else
            {
                sql = "SELECT codigo, nombre, idtabla from sas_datos where idtabla = '" + idtabla + "' " + " and codigo='" + codigo + "' and idempresa= '" + Startup.IdEmpresa + "' order by nombre ";
            }


            
            command.CommandText = sql;

            conn.Open();
            rdr = command.ExecuteReader();

            if ((rdr != null) && (rdr.HasRows))
            {
                while (rdr.Read())
                {

                   

                    searchResults.Add(new sasDatosModel()
                     {
                         codigo = rdr["codigo"].ToString(),
                         descripcion = rdr["nombre"].ToString(),
                         idtabla=rdr["idtabla"].ToString()
                     });
                    //datos = new sasDatosModel
                    //{
                    //    codigo = rdr["codigo"].ToString(),
                    //    descripcion = rdr["nombre"].ToString()

                    //};

                }

            }

                 conn.Close();

            return searchResults;

        


        }

        public string Put(int id_Solicitud, int nro_solicitud, string destino, string desenlace=null)
        {

            SqlConnection conn = null;
            SqlCommand command = null;
            var connectionString = string.Empty;
            var json = string.Empty;
            connectionString = acc.leerConeccion();


            try
            {

                command = new SqlCommand();

                conn = new SqlConnection(connectionString);
                conn.Open();
                //mytrans = conn.BeginTransaction(IsolationLevel.ReadCommitted, "ActualizarEstado");
                //command.Connection = conn;
                //command.Transaction = mytrans;

                //command.Parameters.Clear();
                command.CommandTimeout = 1200;
                command = new SqlCommand("[Sas_Update_ServiceMobile]", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@idempresa", Startup.IdEmpresa));
                command.Parameters.Add(new SqlParameter("@IdServicio", id_Solicitud));
                command.Parameters.Add(new SqlParameter("@NroSolicitud", nro_solicitud));
                command.Parameters.Add(new SqlParameter("@destino", destino));
                if (desenlace != null)
                {
                    command.Parameters.Add(new SqlParameter("@IdDesenlace", desenlace));
                }
              


                command.ExecuteNonQuery();

                //mytrans.Commit();

                return "OK";
            }
            catch (Exception ex)
            {

                return "Error";
            }
            finally
            {
                conn.Close();
            }

        }

        public List<ServiciosModel> GetAllServicios(string codMovil)
        {
            SqlDataReader rdr = null;
            SqlConnection conn = null;
            SqlCommand command = null;
            var connectionString = string.Empty;
            var json = string.Empty;
            var datos = new sasDatosModel();


            var searchResults = new List<ServiciosModel>();

            connectionString = acc.leerConeccion();
            string sql = "";

               try
            {

            conn = new SqlConnection(connectionString);

            command = new SqlCommand();

            command.Connection = conn;

            command.CommandType = CommandType.Text;
            command.CommandTimeout = 1200;

            sql = "select   idSolicitud  ,NroSolicitud  ,fechaLlamada  ,horaLlamada  " + 
                ",idTipoServicio  ,Paciente  ,Telefono  ,isnull(edad,0) edad  ,solicitante  " + 
                ",direccionRef  ,direccionRef2  ,nrocasa  ,referencia  ,Motivo  ,nroSalida  " + 
                ",idMovil  ,chofer  ,paramedico  ,  medico  ,Acompañante  ,observacion  " +
                " ,Estado  ,IdEstado  ,HoraEstado  , " +
                " (select motivo from sas_motivos where idmotivo=SAS_SERVICIOS.IdMotivo1 and idempresa= SAS_SERVICIOS.idempresa ) IdMotivo1 , " +
                " (select motivo from sas_motivos where idmotivo=SAS_SERVICIOS.IdMotivo2 and idempresa= SAS_SERVICIOS.idempresa ) IdMotivo2 , " +
                " (select motivo from sas_motivos where idmotivo=SAS_SERVICIOS.IdMotivo3 and idempresa= SAS_SERVICIOS.idempresa ) IdMotivo3 , " +
                " OtroServicio  ,IdProducto ,destino,iddesenlace, " +
                " (select descripcion from st_Producto where idproducto= SAS_SERVICIOS.IdProducto and idempresa = SAS_SERVICIOS.idempresa) producto " + 
                " from SAS_SERVICIOS " +
                " where idestado <> '009' and (estado not in ('C','A') ) and idmovil = '" + codMovil + "' and idempresa= '" + Startup.IdEmpresa + "' order by estado desc ";
          


            command.CommandText = sql;

            conn.Open();
            rdr = command.ExecuteReader();

            if ((rdr != null) && (rdr.HasRows))
            {
                while (rdr.Read())
                {
                    searchResults.Add(new ServiciosModel()
                    {
                        id_Solicitud = int.Parse(rdr["idSolicitud"].ToString()),
                        NumeroSolicitud = int.Parse(rdr["NroSolicitud"].ToString()),
                        fecha_Llamado = DateTime.Parse(rdr["fechaLlamada"].ToString()),
                        hora_Llamado = rdr["horaLlamada"].ToString(),
                        nombrePaciente = rdr["Paciente"].ToString(),
                        Tel = rdr["Telefono"].ToString(),
                        edadPaciente = decimal.Parse( rdr["edad"].ToString()),
                        nombreSolicitante = rdr["solicitante"].ToString(),
                        direccionReferecia = rdr["direccionRef"].ToString(),
                        direccionReferecia2 = rdr["direccionRef2"].ToString(),
                        numeroCasa = rdr["nrocasa"].ToString(),
                        referencia = rdr["referencia"].ToString(),
                        Motivo = rdr["Motivo"].ToString(),
                        nroSalida = rdr["nroSalida"].ToString(),
                        codMovil = rdr["idMovil"].ToString(),
                        codChofer = rdr["chofer"].ToString(),
                        codParamedico = rdr["paramedico"].ToString(),
                        codMedico = rdr["medico"].ToString(),
                        Acompañante = rdr["Acompañante"].ToString(),
                        observacion = rdr["observacion"].ToString(),
                        Estado = rdr["Estado"].ToString(),
                        codEstado = rdr["IdEstado"].ToString(),
                        HoraEstado = rdr["HoraEstado"].ToString(),
                        codMotivo1 = rdr["IdMotivo1"].ToString(),
                        codMotivo2 = rdr["IdMotivo2"].ToString(),
                        codMotivo3 = rdr["IdMotivo3"].ToString(),
                        OtroMotivo = rdr["OtroServicio"].ToString(),
                        codTipo = rdr["IdProducto"].ToString(),
                        codInstitucion = rdr["destino"].ToString(),
                        codDesenlace = rdr["iddesenlace"].ToString(),
                        producto= rdr["producto"].ToString()
                    });
                    //datos = new sasDatosModel
                    //{
                    //    codigo = rdr["codigo"].ToString(),
                    //    descripcion = rdr["nombre"].ToString()

                    //};

                }

            }


          
            

            }
               catch (Exception ex)
               {
                    searchResults.Add(new ServiciosModel()
                    {
                        
                        nombreSolicitante =ex.Message.ToString()
                    });
               }
               finally
               {
                   conn.Close();
               }

            return searchResults;
        }
    }
}