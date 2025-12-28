using Microsoft.EntityFrameworkCore;
using VAAO_BE.Entities;

namespace VAAO_BE.Data
{

    public partial class VAAOContext : DbContext
    {
        public VAAOContext() { }

        public VAAOContext(DbContextOptions<VAAOContext> options) : base(options) { }

        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Pedidos> Pedidos { get; set; }
        public virtual DbSet<Clientes> Clientes { get; set; }
        public virtual DbSet<Visitas> Visitas { get; set; }
        public virtual DbSet<Entregas> Entregas { get; set; }
        public virtual DbSet<Conservadores> Conservadores { get; set; }
        public virtual DbSet<Repartidores> Repartidores { get; set; }
        public virtual DbSet<Ventas> Ventas { get; set; }
        public virtual DbSet<MetodosPago> MetodosPago { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=db-mysql-nyc3-54924-do-user-31175109-0.l.db.ondigitalocean.com;Port=25060;Database=VAAO;User=doadmin;Password=AVNS_dhvju98ctTc6Q_kfdR5;SslMode=None;",
                ServerVersion.AutoDetect(
                    "Server=db-mysql-nyc3-54924-do-user-31175109-0.l.db.ondigitalocean.com;Port=25060;Database=VAAO;User=doadmin;Password=AVNS_dhvju98ctTc6Q_kfdR5;SslMode=None;"
                )
            );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity => entity.HasKey(K => K.IdUser));
            modelBuilder.Entity<Pedidos>(entity => entity.HasKey(K => K.IdPedido));
            modelBuilder.Entity<Clientes>(entity => entity.HasKey(k => k.IdCliente));
            modelBuilder.Entity<Visitas>(entity => entity.HasKey(k => k.IdVisita));
            modelBuilder.Entity<Entregas>(entity => entity.HasKey(k => k.IdEntrega));
            modelBuilder.Entity<Conservadores>(entity => entity.HasKey(k => k.IdConservador));
            modelBuilder.Entity<Repartidores>(entity => entity.HasKey(k => k.IdRepartidor));
            modelBuilder.Entity<Ventas>(entity => entity.HasKey(k => k.IdVenta));
            modelBuilder.Entity<MetodosPago>(entity => entity.HasKey(k => k.IdMetodoPago));
        }
    }
}
