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
    public class DeviceUserContext : DbContext
    {
        public DeviceUserContext()
            : base("FuturaConnection")
        {
            Database.SetInitializer<DeviceUserContext>(null);
        }

        public virtual DbSet<DeviceUser> DeviceUser { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("dbo");

            //Map entity to table
            modelBuilder.Entity<DeviceUser>().ToTable("DeviceUser");


        }

    }
     [Table("DeviceUser")]
    public class DeviceUser
    {
      
           [Key]
           [Column("UserName")]
         [Display(Name="Usuario")]
        public string usuario { get; set; }
          
         
         [Column("password")]
         [Display(Name = "Clave")]
         [DataType(DataType.Password)]
        public string pass { get; set; }

        [Column("idMovil")]
        [Display(Name = "Código de Movil")]
        public string codMovil { get; set; }
        [Display(Name = "Nombres")]
        public string nombres { get; set; }
        [Display(Name = "Apellidos")]
        public string apellidos { get; set; }
        
   
        public string idRegistro { get; set; }

    }
}