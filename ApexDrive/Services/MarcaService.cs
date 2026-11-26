using System;
using ApexDrive.models;

namespace ApexDrive.services;

public class MarcaService : IMarcaService
{
    private static List<Marca> marcas = new List<Marca>();

    public List<Marca> ListarMarcas()
    {
        return marcas;
    }

    public Marca? ObterPorId(int id)
    {
        return marcas.FirstOrDefault(x => x.Id == id);
    }

    public void Adicionar(Marca novamarca)
    {
        marcas.Add(novamarca);
    }

    public void Atualizar(Marca marcaAtualizado)
    {
        var marca = marcas.FirstOrDefault(x => x.Id == marcaAtualizado.Id);

        if(marca is null)
        {
            return;
        }

        marca.Nome = marcaAtualizado.Nome;
        marca.PaisOrigem = marcaAtualizado.PaisOrigem;
    }
}
