using ApexDrive.Models;

namespace ApexDrive.Services;

public interface IOpcionalRepository
{
	Task<List<Opcional>> ListarAsync();
	Task<Opcional?> ObterAsync(int id);
	Task<bool> ExisteAsync(string nome, string categoria, int? idIgnorado);

	void Adicionar(Opcional opcional);
	void Remover(Opcional opcional);

	Task SalvarAsync();
}
