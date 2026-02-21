using Microsoft.EntityFrameworkCore;

namespace ColdStoreManagement.BLL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Chamber> chamber { get; set; }
        public DbSet<Party> party { get; set; }
        public DbSet<Allocation> Allocation { get; set; }

        public DbSet<Agreement> Agreement { get; set; }
        public DbSet<Dtrans> Dtrans { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<SubGrower> SubGrowers { get; set; }
        public DbSet<UnitMaster> Unit_Master { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dtrans>()
                .HasOne(d => d.party)
                .WithMany()
                .HasForeignKey(d => d.Partyid)
                .OnDelete(DeleteBehavior.Restrict); //  no cascade

            modelBuilder.Entity<Dtrans>()
                .HasOne(d => d.chamber)
                .WithMany()
                .HasForeignKey(d => d.Chamberid)
                .OnDelete(DeleteBehavior.Restrict); //  no cascade

            modelBuilder.Entity<Dtrans>()
                .HasOne(d => d.Allocation)
                .WithMany()
                .HasForeignKey(d => d.AllocationId)
                .OnDelete(DeleteBehavior.Restrict); //  no cascade

            modelBuilder.Entity<Dtrans>()
                .HasOne(d => d.SubGrower)
                .WithMany()
                .HasForeignKey(d => d.SubGrowerId)
                .OnDelete(DeleteBehavior.Restrict); //  no cascade


            modelBuilder.Entity<Allocation>()
                .HasOne(d => d.SubGrower)
                .WithMany()
                .HasForeignKey(d => d.SubGrowerId)
                .OnDelete(DeleteBehavior.Restrict); //  no cascade
        }


    }
}
