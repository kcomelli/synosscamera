using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using shcome.loxone.sensor.EntityObjects;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace shcome.loxone.sensor.AppContext
{
    public partial class SeonsorAppDbContext : DbContext
    {
        public SeonsorAppDbContext()
        {
        }

        public SeonsorAppDbContext(DbContextOptions<SeonsorAppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// flag indicating if the database uses an inmemory db
        /// </summary>
        internal bool IsInMemoryDatabase => this.Database?.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";

        public virtual DbSet<Sensor> sensor { get; set; }
        public virtual DbSet<SensorValue> sensor_value { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<SensorValue>(entity =>
            //{
            //    entity.HasOne(d => d.Sensor)
            //        .WithMany(p => p.SensorValues)
            //        .HasForeignKey(d => d.SensorId)
            //        .HasConstraintName("sensor_value_sensorId_fkey");
            //});

            BuildModels(modelBuilder);
        }

        ///// <inheritdoc/>
        //public override void EnsureSeedData()
        //{
        //    base.EnsureSeedData();

        //    var rolesSeeder = new RolesSeeder(this);
        //    rolesSeeder.SeedData();

        //    var userSeeder = new UserSeeder(this);
        //    userSeeder.SeedData();

        //    var tenantSeeder = new TenantSeeder(this);
        //    tenantSeeder.SeedData();

        //    var appSeeder = new AppSeeder(this);
        //    appSeeder.SeedData();

        //    var planSeeder = new PlanSeeder(this);
        //    planSeeder.SeedData();
        //}

        
    }
}
