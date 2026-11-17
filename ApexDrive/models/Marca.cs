using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApexDrive.models;

public class Marca
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo País de Origem é obrigatório.")]
    public string PaisOrigem { get; set; }
// Enquanto não tive veiculos para relacionar com marcar, não vou precisar dessa propriedade de navegação
//  public ICollection<Veiculo> Veiculos { get; set; }
}
