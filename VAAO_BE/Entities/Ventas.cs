using System.ComponentModel.DataAnnotations.Schema;

namespace VAAO_BE.Entities
{

    [Table("Ventas")]
    public class Ventas
    {
        public int IdVenta { get; set; }

        public int IdPedido { get; set; }

        public int IdMetodoPago { get; set; }

        public DateTime FechaRegistro { get; set; }
    }

}