using Android.OS;
using System;
using Android.Runtime;
using Java.Interop;

namespace sas
{
    public class SasDatosModel : Java.Lang.Object, IParcelable
    {

        public string codigo { get; set; }
        public string descripcion { get; set; }


        /*CONSTRUCTOR*/
        public SasDatosModel(string codigo, string descripcion)
        {
            this.codigo = codigo;
            this.descripcion = descripcion;

        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {

            dest.WriteString(this.codigo);
            dest.WriteString(this.descripcion);
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
                string codigo = source.ReadString();
                string descripcion = source.ReadString();


                return new SasDatosModel(codigo, descripcion);
            }

            Java.Lang.Object[] IParcelableCreator.NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
            #endregion
        }
    }
}   
