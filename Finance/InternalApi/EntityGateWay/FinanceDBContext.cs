using DataModel.DataBase;
using Microsoft.EntityFrameworkCore;

namespace InternalApi.EntityGateWay
{
    public class FinanceDBContext : DbContext
    {
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<MainBoardDTO> MainBoards { get; set; }
        public DbSet<PlotDTO> Plots { get; set; }
        public DbSet<TableDTO> Tables { get; set; }

        public FinanceDBContext(DbContextOptions<FinanceDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            BindingTable(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var entityClrType = entityType.ClrType;
                var entityInterface = entityClrType.GetInterface("IEntity`1");

                if (entityInterface != null)
                {
                    var idProperty = entityType.FindProperty("Id");
                    if (idProperty != null)
                        ConfigureIdProperty(modelBuilder, entityClrType, idProperty.ClrType);
                }
            }
        }

        private void BindingTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlotDTO>()
                .HasOne(p => p.Table)
                .WithOne(t => t.Plot)
                .HasForeignKey<TableDTO>(t => t.PlotId)
                .IsRequired();

            modelBuilder.Entity<TableDTO>()
                .HasOne(t => t.MainBoard)
                .WithOne(b => b.Table)
                .HasForeignKey<MainBoardDTO>(b => b.TableId)
                .IsRequired();
        }

        private void ConfigureIdProperty(ModelBuilder modelBuilder, Type entityType, Type idType)
        {
            if (idType == typeof(long))
            {
                modelBuilder.Entity(entityType)
                    .Property("Id")
                    .ValueGeneratedOnAdd();
            }
            else if (idType == typeof(Guid))
            {
                modelBuilder.Entity(entityType)
                    .Property("Id")
                    .HasDefaultValueSql("gen_random_uuid()");
            }
        }
    }
}
