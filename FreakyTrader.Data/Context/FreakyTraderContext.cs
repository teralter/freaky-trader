using FreakyTrader.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreakyTrader.Data.Context
{
    public class FreakyTraderContext : DbContext
    {
        public FreakyTraderContext()
        {
        }

        public FreakyTraderContext(DbContextOptions<FreakyTraderContext> options) : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionId)
                .ForSqlServerIsClustered(false);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.RepDate)
                .ForSqlServerIsClustered();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(
            //  "Server=localhost;Database=FreakyTrader;Integrated Security=False;User ID=sa;Password=sportbox;"
            //  , options => options.MaxBatchSize(30)
            //  );
            //optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
