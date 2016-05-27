using Android.OS;
using System;
using Android.Runtime;
using Java.Interop;

namespace sas
{
	public class DeviceUserModel : Java.Lang.Object, IParcelable
    {
		public string usuario { get; set; }
		public string pass { get; set; }
		public string codMovil { get; set; }
		public string nombres { get; set; }
		public string apellidos { get; set; }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(this.usuario);
            dest.WriteString(this.pass);
            dest.WriteString(this.codMovil);
            dest.WriteString(this.nombres);
            dest.WriteString(this.apellidos);
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
                string usuario = source.ReadString();
                string pass = source.ReadString();
                string codMovil = source.ReadString();
                string nombres = source.ReadString();
                string apellidos = source.ReadString();
                return new DeviceUserModel(usuario, pass, codMovil, nombres, apellidos);
            }

            Java.Lang.Object[] IParcelableCreator.NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
            #endregion
        }


        public DeviceUserModel(string usuario,
                        string pass,
                         string codMovil,
                         string nombres,
                         string apellidos) : base()
        {
            this.usuario = usuario;
            this.pass = pass;
            this.codMovil = codMovil;
            this.nombres = nombres;
            this.apellidos = apellidos;

        }

        /*CONSTRUCTOR*/
        //public  DeviceUserModel(string usuario,
        //                 string pass,
        //                 string codMovil,
        //                 string nombres,
        //                 string apellidos)
        //{
        //    this.usuario = usuario;
        //    this.pass = pass;
        //    this.codMovil = codMovil;
        //    this.nombres = nombres;
        //    this.apellidos = apellidos;

        //}
        /*CONSTRUCTOR*/
        //public DeviceUserModel(string nombres,
        //                       string codMovil)
                        
                       
        //{
        //    this.codMovil = codMovil;
        //    this.nombres = nombres;
        // }
    }
}

