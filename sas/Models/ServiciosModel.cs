using Android.OS;
using System;
using Android.Runtime;
using Java.Interop;

namespace sas
{
    public class ServiciosModel  : Java.Lang.Object, IParcelable

    {
        public int id_Solicitud { get; set; }
        public int NumeroSolicitud { get; set; }
        public Nullable<System.DateTime> fecha_Llamado { get; set; }
        public string hora_Llamado { get; set; }
        public string cod_TipoServicio { get; set; }
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
        /*CONSTRUCTOR*/
        public ServiciosModel(int id_solicitud, int NumeroSolicitud, DateTime  fecha_Llamado, string hora_Llamado
            , string cod_TipoServicio,string nombrePaciente, string Tel, decimal edadPaciente, string nombreSolicitante,
            string direccionReferecia, string direccionReferecia2, string numeroCasa, string referencia, string Motivo, 
            string nroSalida, string codMovil, string codChofer, string Acompañante, string observacion, string Estado, 
            string codEstado, string HoraEstado, string codMotivo1, string codMotivo2, string codMotivo3, string OtroMotivo
           , string codTipo)
        {
            this.id_Solicitud = id_solicitud;
            this.NumeroSolicitud = NumeroSolicitud;
            this.fecha_Llamado = fecha_Llamado;
            this.hora_Llamado = hora_Llamado;
            this.cod_TipoServicio = cod_TipoServicio;
            this.nombrePaciente = nombrePaciente;
            this.Tel = Tel;
            this.edadPaciente = edadPaciente;
            this.nombreSolicitante = nombreSolicitante;
            this.direccionReferecia = direccionReferecia;
            this.direccionReferecia2 = direccionReferecia2;
            this.numeroCasa = numeroCasa;
            this.referencia = referencia;
            this.Motivo = Motivo;
            this.nroSalida = nroSalida;
            this.codMovil = codMovil;
            this.codChofer = codChofer;
            this.codParamedico = codParamedico;
            this.codMedico = codMedico;
            this.Acompañante = Acompañante;
            this.observacion = observacion;
            this.Estado = Estado;
            this.codEstado = codEstado;
            this.HoraEstado = HoraEstado;
            this.codMotivo1 = codMotivo1;
            this.codMotivo2 = codMotivo2;
            this.codMotivo3 = codMotivo3;
            this.OtroMotivo = OtroMotivo;
            this.codTipo = codTipo;



    }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteInt(id_Solicitud);
            dest.WriteInt(NumeroSolicitud);
            dest.WriteString(this.fecha_Llamado.ToString());
            dest.WriteString(this.hora_Llamado);
            dest.WriteString(this.cod_TipoServicio);
            dest.WriteString(this.nombrePaciente);
            dest.WriteString(this.Tel);
            dest.WriteString(this.edadPaciente.ToString());
            dest.WriteString(this.nombreSolicitante);
            dest.WriteString(this.direccionReferecia);
            dest.WriteString(this.direccionReferecia2);
            dest.WriteString(this.numeroCasa);
            dest.WriteString(this.referencia);
            dest.WriteString(this.Motivo);
            dest.WriteString(this.nroSalida);
            dest.WriteString(this.codMovil);
            dest.WriteString(this.codChofer);
            dest.WriteString(this.codParamedico);
            dest.WriteString(this.codMedico);
            dest.WriteString(this.Acompañante);
            dest.WriteString(this.observacion);
            dest.WriteString(this.Estado);
            dest.WriteString(this.codEstado);
            dest.WriteString(this.HoraEstado);
            dest.WriteString(this.codMotivo1);
            dest.WriteString(this.codMotivo2);
            dest.WriteString(this.codMotivo3);
            dest.WriteString(this.OtroMotivo);
            dest.WriteString(this.codTipo);
        }

        [ExportField("CREATOR")]
        public static IParcelableCreator GetCreator()
        {
            return new MyParcelableCreator();
        }

        // Parcelable.Creator 
        class MyParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            #region IParcelableCreator implementation
            Java.Lang.Object IParcelableCreator.CreateFromParcel(Parcel source)
            {
                int id_solicitud = source.ReadInt();
                int NumeroSolicitud = source.ReadInt();
                string  fecha_Llamado = source.ReadString();
                string hora_Llamado = source.ReadString();
                string cod_TipoServicio = source.ReadString();
                string nombrePaciente = source.ReadString();
                string Tel = source.ReadString();
                string edadPaciente = source.ReadString();
                string nombreSolicitante = source.ReadString();
                string direccionReferecia = source.ReadString();
                string direccionReferecia2 = source.ReadString();
                string numeroCasa = source.ReadString();
                string referencia = source.ReadString();
                string Motivo = source.ReadString();
                string nroSalida = source.ReadString();
                string codMovil = source.ReadString();
                string codChofer = source.ReadString();
                string codParamedico = source.ReadString();
                string codMedico = source.ReadString();
                string Acompañante = source.ReadString();
                string observacion = source.ReadString();
                string  Estado = source.ReadString();
                string codEstado = source.ReadString();
                string HoraEstado = source.ReadString();
                string codMotivo1 = source.ReadString();
                string codMotivo2 = source.ReadString();
                string codMotivo3 = source.ReadString();
                string OtroMotivo = source.ReadString();
                string codTipo = source.ReadString();



                return new ServiciosModel( id_solicitud,  NumeroSolicitud, DateTime.Parse(fecha_Llamado),  hora_Llamado
                ,  cod_TipoServicio,  nombrePaciente,  Tel, decimal.Parse(edadPaciente),  nombreSolicitante,
                 direccionReferecia,  direccionReferecia2,  numeroCasa,  referencia,  Motivo,
                nroSalida,  codMovil,  codChofer,  Acompañante,  observacion,  Estado,
                 codEstado,  HoraEstado,  codMotivo1,  codMotivo2,  codMotivo3,  OtroMotivo
                ,  codTipo);
            }

            Java.Lang.Object[] IParcelableCreator.NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
            #endregion
        }
    }
}

