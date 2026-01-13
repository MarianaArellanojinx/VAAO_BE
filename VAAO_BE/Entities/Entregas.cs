using System.ComponentModel.DataAnnotations.Schema;

namespace VAAO_BE.Entities
{

    [Table("Entregas")]
    public class Entregas
    {
        public int IdEntrega { get; set; }

        public int? IdRepartidor { get; set; }

        public int IdPedido { get; set; }

        public DateTime? FechaEntrega { get; set; }

        public DateTime? HoraInicio { get; set; }

        public DateTime? HoraLlegada { get; set; }

        public DateTime? HoraRegreso { get; set; }

        public int EstatusReparto { get; set; }

        public string? Observaciones { get; set; }

        public string? ImagenConservadorLlegada { get; set; }

        public string? ImagenConservadorSalida { get; set; }

        public string? ImagenIncidenciaConservador { get; set; }
    }
}
