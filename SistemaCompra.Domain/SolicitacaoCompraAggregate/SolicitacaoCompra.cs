using SistemaCompra.Domain.Core;
using SistemaCompra.Domain.Core.Model;
using SistemaCompra.Domain.ProdutoAggregate;
using SistemaCompra.Domain.SolicitacaoCompraAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaCompra.Domain.SolicitacaoCompraAggregate
{
    public class SolicitacaoCompra : Entity
    {
        public UsuarioSolicitante UsuarioSolicitante { get; private set; }
        public NomeFornecedor NomeFornecedor { get; private set; }
        public IList<Item> Itens { get; private set; }
        public DateTime Data { get; private set; }
        public Situacao Situacao { get; private set; }
        public Money TotalGeral => ObterTotal();
        public CondicaoPagamento CondicaoPagamento { get; private set; }

        private SolicitacaoCompra() { }

        public SolicitacaoCompra(string usuarioSolicitante, string nomeFornecedor)
        {
            Id = Guid.NewGuid();
            UsuarioSolicitante = new UsuarioSolicitante(usuarioSolicitante);
            NomeFornecedor = new NomeFornecedor(nomeFornecedor);
            Data = DateTime.Now;
            Situacao = Situacao.Solicitado;
            Itens = new List<Item>();
        }

        public void AdicionarItem(Produto produto, int qtde)
        {
            Itens.Add(new Item(produto, qtde));
        }

        public void RegistrarCompra()
        {
            if (!Itens.Any())
                throw new BusinessRuleException("A solicitação de compra deve possuir itens!");

            if (ObterTotal().Value > 50000)
                CondicaoPagamento = new CondicaoPagamento(30);

            AddEvent(new CompraRegistradaEvent(Id, Itens, ObterTotal().Value));

        }

        private Money ObterTotal()
        {
            Money total = new Money();
            foreach (var item in Itens)
                total = total.Add(item.Subtotal);

            return total;
        }
    }
}
