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
    public class MotivosModelContext : DbContext
    {
        public MotivosModelContext()
            : base("FuturaConnection")
        {
            Database.SetInitializer<MotivosModelContext>(null);
        }

        public virtual DbSet<MotivosModel> MotivosModel { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("dbo");

            //Map entity to table
            modelBuilder.Entity<MotivosModel>().ToTable("sas_Motivos");


        }

       
    }
    [Table("sas_Motivos")]
    public partial class MotivosModel
    {
        [Key]
        [Column("IdMotivo")]
        public string codMotivo { get; set; }
        [Column("Motivo")]
        public string descripcionMotivo { get; set; }
      
        public string IdEmpresa { get; set; }
     

    }
}