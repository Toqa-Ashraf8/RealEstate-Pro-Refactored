using Microsoft.EntityFrameworkCore;
using WebApp1.Models;

namespace WebApp1.EF
{
    public class DataContext :DbContext
    {
     public DataContext(DbContextOptions<DataContext> options) : base(options){}
        public DbSet<Project> Projects { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Negotiation> Negotiations { get; set; }
        public DbSet <ClientExtraDetails> ClientExtraDetails { get; set; }
        public DbSet<Rejected_negotiations_phase> Rejected_negotiations_phases { get; set; }
        public DbSet<Installment>Installments { get; set; }
        public DbSet<User>Users { get; set; }
        public DbSet<UnitBooking> UnitBooking { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Negotiation>()
                .Property(p => p.NegotiationDate)
                .HasColumnType("date");
                 modelBuilder.Entity<Negotiation>()
                .Property(n => n.DiscountAmount)
                .HasColumnType("decimal(5,2)");
             modelBuilder.UseCollation("Arabic_CI_AS");
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties().Where(p => p.ClrType == typeof(string));
                foreach (var property in properties)
                {
                    property.SetIsUnicode(true);
                }
            }
        }
      
    }
}
