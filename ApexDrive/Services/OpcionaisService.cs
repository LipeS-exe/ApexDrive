using ApexDrive.Models;

namespace ApexDrive.Services;

public class OpcionaisService : IOpcionaisService
{
	private readonly IOpcionalRepository _repository;

	public OpcionaisService(IOpcionalRepository repository)
	{
		_repository = repository;
	}

	public async Task<List<Opcional>> ListarAsync()
	{
		return await _repository.ListarAsync();
	}

	public async Task<Opcional?> ObterAsync(int id)
	{
		return await _repository.ObterAsync(id);
	}

	public async Task<Opcional> CriarAsync(Opcional opcional)
	{
		await ValidarRegrasAsync(opcional, idIgnorado: null);

		opcional.Id = 0;

		_repository.Adicionar(opcional);
		await _repository.SalvarAsync();

		return opcional;
	}

	public async Task<bool> AtualizarAsync(int id, Opcional opcionalAtualizado)
	{
		var opcional = await _repository.ObterAsync(id);

		if (opcional is null)
		{
			return false;
		}

		await ValidarRegrasAsync(opcionalAtualizado, idIgnorado: id);

		opcional.Nome = opcionalAtualizado.Nome;
		opcional.Categoria = opcionalAtualizado.Categoria;

		await _repository.SalvarAsync();

		return true;
	}

	public async Task<bool> RemoverAsync(int id)
	{
		var opcional = await _repository.ObterAsync(id);

		if (opcional is null)
		{
			return false;
		}

		_repository.Remover(opcional);
		await _repository.SalvarAsync();

		return true;
	}

	private async Task ValidarRegrasAsync(Opcional opcional, int? idIgnorado)
	{
		if (await _repository.ExisteAsync(opcional.Nome, opcional.Categoria, idIgnorado))
		{
			throw new RegraDeNegocioException("Já existe um opcional com esse nome.");
		}
	}
}
