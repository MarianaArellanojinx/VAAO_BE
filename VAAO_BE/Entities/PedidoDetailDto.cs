namespace VAAO_BE.Entities
{
    public class PedidoDetailDto
    {
        public int IdPedido { get; set; }

        public int IdCliente { get; set; }

        public DateTime FechaPedido { get; set; }

        public DateTime? FechaProgramada { get; set; }

        public int TotalBolsas { get; set; }

        public double PrecioUnitario { get; set; }

        public double TotalPagar { get; set; }

        public int EstatusPedido { get; set; }

        public string? Observaciones { get; set; }

        public int? IdRepartidor { get; set; }

        public string NombreCliente { get; set; } = string.Empty;

        public string? Ubicacion { get; set; }
    }
}
