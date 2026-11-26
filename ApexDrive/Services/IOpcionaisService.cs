using ApexDrive.Models;

namespace ApexDrive.Services;

public interface IOpcionaisService
{
	Task<List<Opcional>> ListarAsync();
	Task<Opcional?> ObterAsync(int id);
	Task<Opcional> CriarAsync(Opcional opcional);
	Task<bool> AtualizarAsync(int id, Opcional opcionalAtualizado);
	Task<bool> RemoverAsync(int id);
}
