using ApexDrive.Models;

namespace ApexDrive.Repositories.Interfaces;

public interface IVeiculoRepository
{
    Task<List<Veiculo>> ListarAsync();
    Task<Veiculo?> ObterPorIdAsync(int id);
    Task<bool> PlacaExisteAsync(string placa, int? idIgnorado);
    Task<bool> MarcaExisteAsync(int marcaId);
    Task<Opcional?> ObterOpcionalAsync(int opcionalId);
    Task AdicionarAsync(Veiculo veiculo);
    void Remover(Veiculo veiculo);
    Task SalvarAsync();
}
