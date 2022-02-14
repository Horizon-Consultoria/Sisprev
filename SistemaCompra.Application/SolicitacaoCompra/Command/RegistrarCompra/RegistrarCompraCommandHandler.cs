using AutoMapper;
using MediatR;
using SistemaCompra.Application.Produto.Query.ObterProduto;
using SistemaCompra.Application.SolicitacaoCompra.Command.RegistrarCompra;
using SistemaCompra.Infra.Data.UoW;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

using SolicitacaoCompraAgg = SistemaCompra.Domain.SolicitacaoCompraAggregate;
using ProdutoAgg = SistemaCompra.Domain.ProdutoAggregate;

namespace SistemaCompra.Application.Produto.Command.RegistrarProduto
{
    public class RegistrarCompraCommandHandler : CommandHandler, IRequestHandler<RegistrarCompraCommand, bool>
    {
        private readonly SolicitacaoCompraAgg.ISolicitacaoCompraRepository solicitacaoCompraRepository;

        private readonly IMediator mediator;

        private readonly IMapper mapper;

        public RegistrarCompraCommandHandler(SolicitacaoCompraAgg.ISolicitacaoCompraRepository solicitacaoCompraRepository, IUnitOfWork uow, IMediator mediator, IMapper mapper) : base(uow, mediator)
        {
            this.solicitacaoCompraRepository = solicitacaoCompraRepository;
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public Task<bool> Handle(RegistrarCompraCommand request, CancellationToken cancellationToken)
        {
            var solicitacaoCompra = new SolicitacaoCompraAgg.SolicitacaoCompra(request.UsuarioSolicitante, request.NomeFornecedor);

            request.Itens.AsParallel().ForAll(item => solicitacaoCompra.AdicionarItem(ObterItem(item).GetAwaiter().GetResult(), 0));

            solicitacaoCompra.RegistrarCompra();
            solicitacaoCompraRepository.RegistrarCompra(solicitacaoCompra);

            Commit();
            PublishEvents(solicitacaoCompra.Events);

            return Task.FromResult(true);
        }

        private async Task<ProdutoAgg.Produto> ObterItem(Guid itemId)
        {
            var item = await mediator.Send(new ObterProdutoQuery() { Id = itemId });

            return mapper.Map<ProdutoAgg.Produto>(item);
        }
    }
}
