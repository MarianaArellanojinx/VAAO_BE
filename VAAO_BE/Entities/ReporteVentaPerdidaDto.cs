namespace VAAO_BE.Entities
{
    public class ReporteVentaPerdidaDetalleDto
    {
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string NombreNegocio { get; set; } = string.Empty;
        public int AnioSemana { get; set; }
        public int NumeroSemana { get; set; }
        public DateTime InicioSemana { get; set; }
        public DateTime FinSemana { get; set; }
        public int BolsasObjetivo { get; set; }
        public int BolsasCompradas { get; set; }
        public int BolsasPerdidas { get; set; }
        public double TotalPagado { get; set; }
        public bool CumplioObjetivo { get; set; }
    }

    public class ReporteVentaPerdidaResumenDto
    {
        public int MetaBolsasPorSemana { get; set; }
        public int TotalNegocios { get; set; }
        public int TotalSemanas { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalBolsasObjetivo { get; set; }
        public int TotalBolsasCompradas { get; set; }
        public int TotalBolsasPerdidas { get; set; }
        public int NegociosConFaltante { get; set; }
        public int NegociosSinCompras { get; set; }
        public double PorcentajeCumplimiento { get; set; }
    }

    public class ReporteVentaPerdidaResponseDto
    {
        public ReporteVentaPerdidaResumenDto Resumen { get; set; } = new();
        public List<ReporteVentaPerdidaDetalleDto> Detalle { get; set; } = [];
    }
}
