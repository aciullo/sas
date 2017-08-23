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
    public class ServiciosContext : DbContext
    {
        public ServiciosContext()
            : base("FuturaConnection")
        {
            Database.SetInitializer<ServiciosContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
      
        }

        public virtual DbSet<ServiciosModel> sas_servicios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("dbo");

            //Map entity to table
            modelBuilder.Entity<ServiciosModel>().ToTable("sas_servicios");


        }
     
    }

  [Table("sas_servicios")]
    public partial class ServiciosModel
        {
            [Key]

            [Column ("idSolicitud")] 
            public int id_Solicitud { get; set; }
         
             [Column ("NroSolicitud")] 
            public Nullable<int> NumeroSolicitud { get; set; }

            [Column("fechaLlamada")] 
            public Nullable<System.DateTime> fecha_Llamado { get; set; }
        
            [Column("horaLlamada")] 
            public string hora_Llamado { get; set; }

                  
            [Column("Paciente")] 
            public string nombrePaciente { get; set; }
            
            [Column("Telefono")] 
            public string Tel { get; set; }
            
            [Column("edad")] 
            public Nullable<decimal> edadPaciente { get; set; }

            [Column("solicitante")] 
            public string nombreSolicitante { get; set; }
            
            [Column("direccionRef")] 
            public string direccionReferecia { get; set; }

            [Column("direccionRef2")] 
            public string direccionReferecia2 { get; set; }

            [Column("nrocasa")] 
            public string numeroCasa { get; set; }

            [Column("referencia")] 
            public string referencia { get; set; }
          
            public string Motivo { get; set; }
         
            public string nroSalida { get; set; }

            [Column("idMovil")] 
            public string codMovil { get; set; }

            [Column("chofer")] 
            public string codChofer { get; set; }
            [Column("paramedico")] 
            public string codParamedico { get; set; }
            [Column("medico")] 
            public string codMedico { get; set; }
            public string Acompañante { get; set; }
           public string observacion { get; set; }
            public string Estado { get; set; }
                        
            [Column("IdEstado")] 
            public string codEstado { get; set; }
            public string HoraEstado { get; set; }

            [Column("IdMotivo1")] 
            public string codMotivo1 { get; set; }
      
            [Column("IdMotivo2")] 
            public string codMotivo2 { get; set; }
            [Column("IdMotivo3")] 
            public string codMotivo3 { get; set; }
           [Column("OtroServicio")] 
            public string OtroMotivo { get; set; }
           [Column("IdProducto")]
           public string codTipo { get; set; }

          

           [Column("destino")]
           public string codInstitucion { get; set; }

           [Column("iddesenlace")]
           public string codDesenlace { get; set; }

           public string idEmpresa { get; set; }

           

            public string sv_ta { get; set; }
            public string sv_fc { get; set; }
            public string sv_tempe { get; set; }
            public string sv_fresp { get; set; }
            public string SAT { get; set; }
            public string Glasgow { get; set; }
            public string Glicemia { get; set; }

            public string IndicacionArribo { get; set; }
            [NotMapped]
            public string producto { get; set; }

         
        }



    }
