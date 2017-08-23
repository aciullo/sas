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
    public class ABMServiciosContext : DbContext
    {
        public ABMServiciosContext()
            : base("FuturaConnection")
        {
            Database.SetInitializer<ABMServiciosContext>(null);
        }

        public virtual DbSet<ABMServicios> sas_servicios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("dbo");

            //Map entity to table
            modelBuilder.Entity<ABMServicios>().ToTable("sas_servicios");


        }

    }
 
    
    [Table("sas_servicios")]
    public class ABMServicios
    {

        [Key]

        [Column("idSolicitud")] 
        public int id_Solicitud { get; set; }
      
        [Column("NroSolicitud")] 
        public Nullable<int> NumeroSolicitud { get; set; }

        [Column("idMovil")] 
        public string codMovil { get; set; }

        [Column("Destino")] 
        public string codServicioFinal { get; set; }

       [Column("IdProductoFinal")] 
        public string codProductoFinal { get; set; }
    }
}