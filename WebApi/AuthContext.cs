using Microsoft.AspNet.Identity.EntityFramework;
using sas_Futura.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace sas_Futura
{
    public class AuthContext: IdentityDbContext<IdentityUser>
    {
        public AuthContext()
            : base("FuturaConnection")
        {
          //  Database.SetInitializer<AuthContext>(null);
        }
        public virtual DbSet<DeviceUser> DeviceUser { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //Configure default schema
        //    modelBuilder.HasDefaultSchema("dbo");

        //    //Map entity to table
        //    modelBuilder.Entity<DeviceUser>().ToTable("DeviceUser");


        //}
    
    }

}