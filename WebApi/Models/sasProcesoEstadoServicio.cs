using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace sas_Futura.Models
{


    public class sasProcesoEstadoServicioContext : DbContext
    {
        public sasProcesoEstadoServicioContext()
            : base("FuturaConnection")
        {
            Database.SetInitializer<sasProcesoEstadoServicioContext>(null);
        }

        public virtual DbSet<sasProcesoEstadoServicio> sasProcesoEstadoServicio { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("dbo");

            //Map entity to table
            modelBuilder.Entity<sasProcesoEstadoServicio>().ToTable("sas_ProcesoEstadoServicio");


        }

    }
    
    [Table("sas_ProcesoEstadoServicio")]
    public class sasProcesoEstadoServicio
    {
        [Key]
        public int idLog { get; set; }
        public Nullable<int> id_solicitud { get; set; }
        public Nullable<int> NumeroSolicitud { get; set; }
        public string Nombre { get; set; }
        public string codMovil { get; set; }
        public string codEstado { get; set; }
        public string HoraEstado { get; set; }
        public string codInstitucion { get; set; }
        public string codDesenlace { get; set; }
        public string AuditUsuario { get; set; }
        public string AuditId { get; set; }
        public string GeoData { get; set; }
        public string Address { get; set; }

        public string IdEmpresa { get;set;}

        public string sv_ta { get; set; }
        public string sv_fc { get; set; }
        public string sv_tempe { get; set; }
        public string sv_fresp { get; set; }
        public string SAT { get; set; }
        public string Glasgow { get; set; }
        public string Glicemia { get; set; }
        public string IndicacionArribo { get; set; }
    }
}