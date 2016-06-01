using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite.Net.Attributes;

namespace sas.Models
{
    public class ServiciosLocalModel
    {
        [PrimaryKey, AutoIncrement]
        public int id_registro { get; set; }
        public int id_Solicitud { get; set; }
        public int NumeroSolicitud { get; set; }
        public DateTime fecha_Llamado { get; set; }
        public string hora_Llamado { get; set; }
        public string nombrePaciente { get; set; }
        public string Tel { get; set; }
        public decimal edadPaciente { get; set; }
        public string nombreSolicitante { get; set; }
        public string direccionReferecia { get; set; }
        public string direccionReferecia2 { get; set; }
        public string numeroCasa { get; set; }
        public string referencia { get; set; }
        public string Motivo { get; set; }
        public string nroSalida { get; set; }
        public string codMovil { get; set; }
        public string codChofer { get; set; }
        public string codParamedico { get; set; }
        public string codMedico { get; set; }
        public string Acompañante { get; set; }
        public string observacion { get; set; }
        public string Estado { get; set; }
        public string codEstado { get; set; }
        public string HoraEstado { get; set; }
        public string codMotivo1 { get; set; }
        public string codMotivo2 { get; set; }
        public string codMotivo3 { get; set; }
        public string OtroMotivo { get; set; }
        public string codTipo { get; set; }
        public string codInstitucion { get; set; }
        public string codDesenlace { get; set; }
        public string producto { get; set; }

        //public ServiciosLocalModel()
        //{
        //}
        //public ServiciosLocalModel(int id_solicitud, int NumeroSolicitud, DateTime fecha_Llamado, string hora_Llamado
        //    , string nombrePaciente, string Tel, decimal edadPaciente, string nombreSolicitante,
        //    string direccionReferecia, string direccionReferecia2, string numeroCasa, string referencia, string Motivo,
        //    string nroSalida, string codMovil, string codChofer, string Acompañante, string observacion, string Estado,
        //    string codEstado, string HoraEstado, string codMotivo1, string codMotivo2, string codMotivo3, string OtroMotivo
        //   , string codTipo, string codInstitucion, string codDesenlace, string producto)
        //{
        //    this.id_Solicitud = id_solicitud;
        //    this.NumeroSolicitud = NumeroSolicitud;
        //    this.fecha_Llamado = fecha_Llamado;
        //    this.hora_Llamado = hora_Llamado;
        //    this.nombrePaciente = nombrePaciente;
        //    this.Tel = Tel;
        //    this.edadPaciente = edadPaciente;
        //    this.nombreSolicitante = nombreSolicitante;
        //    this.direccionReferecia = direccionReferecia;
        //    this.direccionReferecia2 = direccionReferecia2;
        //    this.numeroCasa = numeroCasa;
        //    this.referencia = referencia;
        //    this.Motivo = Motivo;
        //    this.nroSalida = nroSalida;
        //    this.codMovil = codMovil;
        //    this.codChofer = codChofer;
        //    this.codParamedico = codParamedico;
        //    this.codMedico = codMedico;
        //    this.Acompañante = Acompañante;
        //    this.observacion = observacion;
        //    this.Estado = Estado;
        //    this.codEstado = codEstado;
        //    this.HoraEstado = HoraEstado;
        //    this.codMotivo1 = codMotivo1;
        //    this.codMotivo2 = codMotivo2;
        //    this.codMotivo3 = codMotivo3;
        //    this.OtroMotivo = OtroMotivo;
        //    this.codTipo = codTipo;
        //    this.codInstitucion = codInstitucion;
        //    this.codDesenlace = codDesenlace;
        //    this.producto = producto;
        //}
    }
}