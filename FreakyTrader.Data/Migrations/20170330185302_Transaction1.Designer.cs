using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using FreakyTrader.Data.Context;

namespace FreakyTrader.Data.Migrations
{
    [DbContext(typeof(FreakyTraderContext))]
    [Migration("20170330185302_Transaction1")]
    partial class Transaction1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FreakyTrader.Data.Entities.Stock", b =>
                {
                    b.Property<int>("StockId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasMaxLength(10);

                    b.Property<int>("ExtId");

                    b.Property<int>("MarketId");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("StockId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("FreakyTrader.Data.Entities.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Price");

                    b.Property<DateTime>("RepDate");

                    b.Property<int>("StockId");

                    b.Property<int>("Volume");

                    b.HasKey("TransactionId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("RepDate")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("StockId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("FreakyTrader.Data.Entities.Transaction", b =>
                {
                    b.HasOne("FreakyTrader.Data.Entities.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
