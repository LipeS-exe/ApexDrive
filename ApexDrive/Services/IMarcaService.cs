using System;
using ApexDrive.models;

namespace ApexDrive.services;

public interface IMarcaService
{
    List<Marca> ListarMarcas();
    Marca? ObterPorId(int id);
    void Adicionar(Marca novamarca);
    void Atualizar(Marca marcaAtualizado);
}
