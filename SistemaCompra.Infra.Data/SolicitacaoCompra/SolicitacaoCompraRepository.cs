using SistemaCompra.Domain.SolicitacaoCompraAggregate;
using SolicitacaoCompraAggr = SistemaCompra.Domain.SolicitacaoCompraAggregate;

namespace SistemaCompra.Infra.Data.SolicitacaoCompra
{
    public class SolicitacaoCompraRepository : ISolicitacaoCompraRepository
    {
        private readonly SistemaCompraContext context;

        public SolicitacaoCompraRepository(SistemaCompraContext context)  {
            this.context = context;
        }

        public void RegistrarCompra(SolicitacaoCompraAggr.SolicitacaoCompra solicitacaoCompra)
        {
            context.Set<SolicitacaoCompraAggr.SolicitacaoCompra>().Add(solicitacaoCompra);
        }
    }
}
