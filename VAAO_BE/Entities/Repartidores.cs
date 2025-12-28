using System.ComponentModel.DataAnnotations.Schema;

namespace VAAO_BE.Entities
{

    [Table("Repartidores")]
    public class Repartidores
    {
        public int IdRepartidor { get; set; }

        public string NombreRepartidor { get; set; } = null!;

        public string ApellidoRepartidor { get; set; } = null!;

        public DateTime AltaRepartidor { get; set; }

        public DateTime? BajaRepartidor { get; set; }
    }
}
