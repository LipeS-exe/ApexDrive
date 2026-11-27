using System.ComponentModel.DataAnnotations;
using ApexDrive.models;

namespace ApexDrive.Models;

public class Veiculo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O campo Modelo é obrigatório.")]
    public string Modelo { get; set; } = string.Empty;

    public int AnoFabricacao { get; set; }

    public int AnoModelo { get; set; }

    [Required(ErrorMessage = "O campo Placa é obrigatório.")]
    [StringLength(7, MinimumLength = 7, ErrorMessage = "A placa deve ter 7 caracteres, sem traço (ex.: ABC1D23).")]
    public string Placa { get; set; } = string.Empty;

    public decimal Preco { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "A quilometragem não pode ser negativa.")]
    public int Quilometragem { get; set; }

    // Situação do carro: "Disponivel", "Reservado" ou "Vendido"
    public string Status { get; set; } = "Disponivel";

    public DateTime DataCadastro { get; set; }

    // N:1 com Marca -> cada veículo pertence a uma marca (MarcaId é a chave estrangeira)
    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }

    // N:N com Opcional -> o EF cria a tabela de ligação VeiculoOpcional
    public ICollection<Opcional> Opcionais { get; set; } = new List<Opcional>();

    // 1:N com PropostaVenda -> liberar quando o Integrante 5 criar a classe PropostaVenda
    // public ICollection<PropostaVenda> Propostas { get; set; } = new List<PropostaVenda>();
}
