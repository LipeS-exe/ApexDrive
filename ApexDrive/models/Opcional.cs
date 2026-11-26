namespace ApexDrive.Models
{
	public class Opcional
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Categoria { get; set; }

		// N:N com Veiculo (lado do Opcional)
		public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
	}
}
