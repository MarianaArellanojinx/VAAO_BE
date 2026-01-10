namespace VAAO_BE.Entities
{
    public class VentaView
    {
        public int IdVenta { get; set; }
        public int IdPedido { get; set; }

        public string NombreCliente { get; set; } = null!;
        public string NombreRepartidor { get; set; } = null!;

        public int BolsasCompradas { get; set; }
        public double TotalPagar { get; set; }

        public DateTime? FechaEntrega { get; set; }
        public string MetodoPago { get; set; } = null!;
    }
}
