using ApexDrive.models;
using ApexDrive.Models.Enums;

namespace ApexDrive.Models;

public class Veiculo
{
    public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public int AnoFabricacao { get; set; }
    public int AnoModelo { get; set; }
    public string Placa { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quilometragem { get; set; }
    public StatusVeiculo Status { get; set; }
    public DateTime DataCadastro { get; set; }

    // N:1 com Marca -> cada veículo pertence a uma marca (MarcaId é a chave estrangeira)
    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }

    // N:N com Opcional -> o EF cria a tabela de ligação VeiculoOpcional
    public ICollection<Opcional> Opcionais { get; set; } = new List<Opcional>();

    // 1:N com PropostaVenda -> liberar quando o Integrante 5 criar a classe PropostaVenda
    // public ICollection<PropostaVenda> Propostas { get; set; } = new List<PropostaVenda>();
}
