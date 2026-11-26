using ApexDrive.Data;
using ApexDrive.Models;
using ApexDrive.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApexDrive.Repositories.Implementations;

// Única classe do módulo que conversa com o banco (através do AppDbContext)
public class VeiculoRepository : IVeiculoRepository
{
    private readonly AppDbContext _context;

    public VeiculoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Veiculo>> ListarAsync()
    {
        // Include traz junto a Marca (N:1) e os Opcionais (N:N)
        return await _context.Veiculos
            .Include(v => v.Marca)
            .Include(v => v.Opcionais)
            .ToListAsync();
    }

    public async Task<Veiculo?> ObterPorIdAsync(int id)
    {
        return await _context.Veiculos
            .Include(v => v.Marca)
            .Include(v => v.Opcionais)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<bool> PlacaExisteAsync(string placa, int? idIgnorado)
    {
        // idIgnorado serve para o PUT: o próprio carro pode manter a placa dele
        return await _context.Veiculos
            .AnyAsync(v => v.Placa == placa && v.Id != idIgnorado);
    }

    public async Task<bool> MarcaExisteAsync(int marcaId)
    {
        return await _context.Marcas.AnyAsync(m => m.Id == marcaId);
    }

    public async Task<Opcional?> ObterOpcionalAsync(int opcionalId)
    {
        return await _context.Opcionais.FirstOrDefaultAsync(o => o.Id == opcionalId);
    }

    public async Task AdicionarAsync(Veiculo veiculo)
    {
        await _context.Veiculos.AddAsync(veiculo);
    }

    public void Remover(Veiculo veiculo)
    {
        _context.Veiculos.Remove(veiculo);
    }

    public async Task SalvarAsync()
    {
        await _context.SaveChangesAsync();
    }
}
