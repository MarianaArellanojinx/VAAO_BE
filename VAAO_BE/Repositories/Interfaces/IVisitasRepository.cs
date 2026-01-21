using VAAO_BE.Entities;

namespace VAAO_BE.Repositories.Interfaces
{
    public interface IVisitasRepository
    {
        public Task<object> GetVisitas(DateTime day);
        public Task InsertVisita(Visitas payload);
    }
}
