using System.ComponentModel.DataAnnotations;

namespace ApexDrive.Dtos;

// Dados que o cliente manda no POST e no PUT.
// Id, Status e DataCadastro ficam de fora porque quem define é a API.
public class VeiculoDto
{
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

    public int MarcaId { get; set; }
}
