using ApexDrive.Models;
using ApexDrive.Repositories.Interfaces;

namespace ApexDrive.Services;

// Aqui ficam as regras de negócio. O acesso ao banco é pedido ao Repository.
public class VeiculoService : IVeiculoService
{
    private readonly IVeiculoRepository _repository;

    public VeiculoService(IVeiculoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Veiculo>> ListarAsync()
    {
        return await _repository.ListarAsync();
    }

    public async Task<Veiculo?> ObterPorIdAsync(int id)
    {
        return await _repository.ObterPorIdAsync(id);
    }

    public async Task<Veiculo> CriarAsync(Veiculo veiculo)
    {
        veiculo.Placa = veiculo.Placa.Trim().ToUpper();
        await ValidarRegrasAsync(veiculo, idIgnorado: null);

        veiculo.Id = 0;                              // quem gera o Id é o banco
        veiculo.Status = "Disponivel";               // todo carro novo entra disponível
        veiculo.DataCadastro = DateTime.UtcNow;      // PostgreSQL exige data em UTC
        veiculo.Marca = null;                        // a marca vem só pelo MarcaId
        veiculo.Opcionais = new List<Opcional>();    // opcionais entram pelo endpoint de vincular

        await _repository.AdicionarAsync(veiculo);
        await _repository.SalvarAsync();

        // Busca de novo para devolver o carro já com a Marca preenchida
        var criado = await _repository.ObterPorIdAsync(veiculo.Id);
        return criado ?? veiculo;
    }

    public async Task<bool> AtualizarAsync(int id, Veiculo veiculoAtualizado)
    {
        var veiculo = await _repository.ObterPorIdAsync(id);

        if (veiculo is null)
        {
            return false;
        }

        veiculoAtualizado.Placa = veiculoAtualizado.Placa.Trim().ToUpper();
        await ValidarRegrasAsync(veiculoAtualizado, idIgnorado: id);

        // Copia só os campos que podem mudar (Id, Status e DataCadastro ficam como estão)
        veiculo.Modelo = veiculoAtualizado.Modelo;
        veiculo.AnoFabricacao = veiculoAtualizado.AnoFabricacao;
        veiculo.AnoModelo = veiculoAtualizado.AnoModelo;
        veiculo.Placa = veiculoAtualizado.Placa;
        veiculo.Preco = veiculoAtualizado.Preco;
        veiculo.Quilometragem = veiculoAtualizado.Quilometragem;
        veiculo.MarcaId = veiculoAtualizado.MarcaId;

        await _repository.SalvarAsync();
        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var veiculo = await _repository.ObterPorIdAsync(id);

        if (veiculo is null)
        {
            return false;
        }

        _repository.Remover(veiculo);
        await _repository.SalvarAsync();
        return true;
    }

    public async Task<bool> VincularOpcionalAsync(int veiculoId, int opcionalId)
    {
        var veiculo = await _repository.ObterPorIdAsync(veiculoId);
        var opcional = await _repository.ObterOpcionalAsync(opcionalId);

        if (veiculo is null || opcional is null)
        {
            return false;
        }

        // RN04: o mesmo opcional não pode entrar duas vezes no mesmo carro
        if (veiculo.Opcionais.Any(o => o.Id == opcionalId))
        {
            throw new RegraDeNegocioException("Esse opcional já está vinculado a este veículo.");
        }

        veiculo.Opcionais.Add(opcional);   // o EF grava uma linha na tabela VeiculoOpcional
        await _repository.SalvarAsync();
        return true;
    }

    public async Task<bool> DesvincularOpcionalAsync(int veiculoId, int opcionalId)
    {
        var veiculo = await _repository.ObterPorIdAsync(veiculoId);

        if (veiculo is null)
        {
            return false;
        }

        var opcional = veiculo.Opcionais.FirstOrDefault(o => o.Id == opcionalId);

        if (opcional is null)
        {
            return false;
        }

        veiculo.Opcionais.Remove(opcional);   // o EF apaga a linha da tabela VeiculoOpcional
        await _repository.SalvarAsync();
        return true;
    }

    private async Task ValidarRegrasAsync(Veiculo veiculo, int? idIgnorado)
    {
        // RN02: preço maior que zero
        if (veiculo.Preco <= 0)
        {
            throw new RegraDeNegocioException("O preço deve ser maior que zero.");
        }

        // RN02: ano de fabricação até o ano que vem
        int anoLimite = DateTime.UtcNow.Year + 1;
        if (veiculo.AnoFabricacao < 1900 || veiculo.AnoFabricacao > anoLimite)
        {
            throw new RegraDeNegocioException($"O ano de fabricação deve estar entre 1900 e {anoLimite}.");
        }

        // Ano do modelo: igual ao de fabricação ou o ano seguinte
        if (veiculo.AnoModelo < veiculo.AnoFabricacao || veiculo.AnoModelo > veiculo.AnoFabricacao + 1)
        {
            throw new RegraDeNegocioException("O ano do modelo deve ser igual ao ano de fabricação ou o ano seguinte.");
        }

        if (!await _repository.MarcaExisteAsync(veiculo.MarcaId))
        {
            throw new RegraDeNegocioException("Marca não encontrada.");
        }

        // RN01: placa não pode repetir
        if (await _repository.PlacaExisteAsync(veiculo.Placa, idIgnorado))
        {
            throw new RegraDeNegocioException("Já existe um veículo com essa placa.");
        }
    }
}
