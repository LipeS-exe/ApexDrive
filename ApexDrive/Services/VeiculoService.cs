using ApexDrive.Dtos;
using ApexDrive.Models;
using ApexDrive.Models.Enums;
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

    public async Task<Veiculo> CriarAsync(VeiculoDto dto)
    {
        dto.Placa = dto.Placa.Trim().ToUpper();
        await ValidarRegrasAsync(dto, idIgnorado: null);

        var veiculo = new Veiculo
        {
            Modelo = dto.Modelo,
            AnoFabricacao = dto.AnoFabricacao,
            AnoModelo = dto.AnoModelo,
            Placa = dto.Placa,
            Preco = dto.Preco,
            Quilometragem = dto.Quilometragem,
            MarcaId = dto.MarcaId,
            Status = StatusVeiculo.Disponivel,   // todo carro novo entra disponível
            DataCadastro = DateTime.UtcNow       // PostgreSQL exige data em UTC
        };

        await _repository.AdicionarAsync(veiculo);
        await _repository.SalvarAsync();

        // Busca de novo para devolver o carro já com a Marca preenchida
        var criado = await _repository.ObterPorIdAsync(veiculo.Id);
        return criado ?? veiculo;
    }

    public async Task<bool> AtualizarAsync(int id, VeiculoDto dto)
    {
        var veiculo = await _repository.ObterPorIdAsync(id);

        if (veiculo is null)
        {
            return false;
        }

        dto.Placa = dto.Placa.Trim().ToUpper();
        await ValidarRegrasAsync(dto, idIgnorado: id);

        veiculo.Modelo = dto.Modelo;
        veiculo.AnoFabricacao = dto.AnoFabricacao;
        veiculo.AnoModelo = dto.AnoModelo;
        veiculo.Placa = dto.Placa;
        veiculo.Preco = dto.Preco;
        veiculo.Quilometragem = dto.Quilometragem;
        veiculo.MarcaId = dto.MarcaId;

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

    private async Task ValidarRegrasAsync(VeiculoDto dto, int? idIgnorado)
    {
        // RN02: preço maior que zero
        if (dto.Preco <= 0)
        {
            throw new RegraDeNegocioException("O preço deve ser maior que zero.");
        }

        // RN02: ano de fabricação até o ano que vem
        int anoLimite = DateTime.UtcNow.Year + 1;
        if (dto.AnoFabricacao < 1900 || dto.AnoFabricacao > anoLimite)
        {
            throw new RegraDeNegocioException($"O ano de fabricação deve estar entre 1900 e {anoLimite}.");
        }

        // Ano do modelo: igual ao de fabricação ou o ano seguinte
        if (dto.AnoModelo < dto.AnoFabricacao || dto.AnoModelo > dto.AnoFabricacao + 1)
        {
            throw new RegraDeNegocioException("O ano do modelo deve ser igual ao ano de fabricação ou o ano seguinte.");
        }

        if (!await _repository.MarcaExisteAsync(dto.MarcaId))
        {
            throw new RegraDeNegocioException("Marca não encontrada.");
        }

        // RN01: placa não pode repetir
        if (await _repository.PlacaExisteAsync(dto.Placa, idIgnorado))
        {
            throw new RegraDeNegocioException("Já existe um veículo com essa placa.");
        }
    }
}
