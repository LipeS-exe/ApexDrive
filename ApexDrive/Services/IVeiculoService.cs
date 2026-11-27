using ApexDrive.Models;

namespace ApexDrive.Services;

public interface IVeiculoService
{
    Task<List<Veiculo>> ListarAsync();
    Task<Veiculo?> ObterPorIdAsync(int id);
    Task<Veiculo> CriarAsync(Veiculo veiculo);
    Task<bool> AtualizarAsync(int id, Veiculo veiculoAtualizado);
    Task<bool> RemoverAsync(int id);
    Task<bool> VincularOpcionalAsync(int veiculoId, int opcionalId);
    Task<bool> DesvincularOpcionalAsync(int veiculoId, int opcionalId);
}
