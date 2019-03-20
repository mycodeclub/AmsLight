using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AmsLight.Models
{
    public class AmsDbContext : DbContext
    {
        public AmsDbContext() : base("name=AmsConStr")
        {
            Database.SetInitializer<AmsDbContext>(null); //disable initializer i.e. disable code first migrations   
        }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Login> Logins { get; set; }

        public System.Data.Entity.DbSet<AmsLight.Models.TrainingCenter> TrainingCenters { get; set; }
    }
}