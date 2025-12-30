using System.ComponentModel.DataAnnotations.Schema;

namespace VAAO_BE.Entities
{

    [Table("Clientes")]
    public class Clientes
    {
        public int IdCliente { get; set; }

        public string NombreNegocio { get; set; } = null!;

        public string NombreCliente { get; set; } = null!;

        public string Calle { get; set; } = null!;

        public string Colonia { get; set; } = null!;

        public string NumeroExterior { get; set; } = null!;

        public string Cp { get; set; } = null!;

        public string Telefono { get; set; } = null!;

        public int Conservadores { get; set; }
        public DateTime FechaAlta { get; set; }

        public DateTime? FechaBaja { get; set; }
        public int IdUser { get; set; }
    }
}
