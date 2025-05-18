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
            modelBuilder.Entity<TableDTO>()
                .HasMany(t => t.Plots)
                .WithOne(p => p.Table)
                .HasForeignKey(p => p.TableId)
                .IsRequired();

            modelBuilder.Entity<MainBoardDTO>()
                .HasOne(b => b.Table)
                .WithOne(t => t.MainBoard)
                .HasForeignKey<MainBoardDTO>(b => b.TableId)
                .IsRequired();

            modelBuilder.Entity<MainBoardDTO>()
                .HasIndex(b => b.TableId)
                .IsUnique();
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
