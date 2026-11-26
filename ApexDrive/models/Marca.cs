using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApexDrive.Models;

namespace ApexDrive.models;

public class Marca
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo País de Origem é obrigatório.")]
    public string PaisOrigem { get; set; }
    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
