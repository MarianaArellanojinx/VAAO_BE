using System.ComponentModel.DataAnnotations.Schema;

namespace VAAO_BE.Entities
{

    [Table("MetodosPago")]
    public class MetodosPago
    {
        public int IdMetodoPago { get; set; }

        public string Descripcion{ get; set; } = null!;
    }
}
