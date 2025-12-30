namespace VAAO_BE.Entities
{
    public class VentaView
    {
        public int IdVenta { get; set; }
        public int IdPedido { get; set; }

        public string NombreCliente { get; set; }
        public string NombreRepartidor { get; set; }

        public int BolsasCompradas { get; set; }
        public double TotalPagar { get; set; }

        public DateTime? FechaEntrega { get; set; }
        public string MetodoPago { get; set; }
    }
}
