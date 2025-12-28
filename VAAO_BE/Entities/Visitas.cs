using System.ComponentModel.DataAnnotations.Schema;

namespace VAAO_BE.Entities
{

    [Table("Visitas")]
    public class Visitas
    {
        public int IdVisita { get; set; }

        public int IdCliente { get; set; }

        public int IdConservador { get; set; }

        public DateTime FechaVisita { get; set; }

    }
}
