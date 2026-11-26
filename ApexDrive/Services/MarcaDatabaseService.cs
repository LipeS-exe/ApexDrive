using System;
using ApexDrive.Data;
using ApexDrive.models;

namespace ApexDrive.services;

public class MarcaDatabaseService : IMarcaService
{
    private AppDbContext _banco;
    public MarcaDatabaseService(AppDbContext _banco)
    {
        this._banco = _banco;
    }

    public List<Marca> ListarMarcas()
    {
        return _banco.Marcas.ToList();
    }

    public Marca? ObterPorId(int id)
    {
        return _banco.Marcas.FirstOrDefault(x => x.Id == id);
    }

    public void Adicionar(Marca novamarca)
    {
        _banco.Marcas.Add(novamarca);
        _banco.SaveChanges();
    }

    public void Atualizar(Marca marcaAtualizado)
    {
        var marca = _banco.Marcas.FirstOrDefault(x => x.Id == marcaAtualizado.Id);

        if (marca is null)
        {
            return;
        }

        marca.Nome = marcaAtualizado.Nome;
        marca.PaisOrigem = marcaAtualizado.PaisOrigem;
        _banco.SaveChanges();
    }
}
