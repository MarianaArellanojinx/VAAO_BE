using System.ComponentModel.DataAnnotations.Schema;

namespace VAAO_BE.Entities
{

    [Table("Conservadores")]
    public class Conservadores
    {
        public int IdConservador { get; set; }

        public int IdCliente { get; set; }

        public string SerialNumber { get; set; } = null!;

        public int EstatusConservador { get; set; }
    }
}
