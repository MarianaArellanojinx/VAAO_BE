using System.ComponentModel.DataAnnotations.Schema;

namespace VAAO_BE.Entities
{

    [Table("Cliente_Conservador")]
    public class Cliente_Conservador
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdConservador { get; set; }
    }
}
