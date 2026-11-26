using ApexDrive.Dtos;
using ApexDrive.Models;
using ApexDrive.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApexDrive.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VeiculosController : ControllerBase
{
    private readonly IVeiculoService _service;

    // Injeção de dependência: o ASP.NET entrega o Service pronto
    public VeiculosController(IVeiculoService service)
    {
        _service = service;
    }

    // GET api/veiculos -> 200
    [HttpGet]
    public async Task<ActionResult<List<Veiculo>>> GetVeiculos()
    {
        var veiculos = await _service.ListarAsync();
        return Ok(veiculos);
    }

    // GET api/veiculos/5 -> 200 ou 404
    [HttpGet("{id}")]
    public async Task<ActionResult<Veiculo>> GetVeiculo(int id)
    {
        var veiculo = await _service.ObterPorIdAsync(id);

        if (veiculo is null)
        {
            return NotFound("Veículo não encontrado.");
        }

        return Ok(veiculo);
    }

    // POST api/veiculos -> 201 ou 400
    [HttpPost]
    public async Task<ActionResult<Veiculo>> PostVeiculo(VeiculoDto dto)
    {
        try
        {
            var criado = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(GetVeiculo), new { id = criado.Id }, criado);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT api/veiculos/5 -> 204, 404 ou 400
    [HttpPut("{id}")]
    public async Task<IActionResult> PutVeiculo(int id, VeiculoDto dto)
    {
        try
        {
            var atualizado = await _service.AtualizarAsync(id, dto);

            if (!atualizado)
            {
                return NotFound("Veículo não encontrado.");
            }

            return NoContent();
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE api/veiculos/5 -> 204 ou 404
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVeiculo(int id)
    {
        var removido = await _service.RemoverAsync(id);

        if (!removido)
        {
            return NotFound("Veículo não encontrado.");
        }

        return NoContent();
    }

    // POST api/veiculos/5/opcionais/2 -> vincula o opcional 2 ao carro 5 (N:N)
    [HttpPost("{veiculoId}/opcionais/{opcionalId}")]
    public async Task<IActionResult> VincularOpcional(int veiculoId, int opcionalId)
    {
        try
        {
            var vinculado = await _service.VincularOpcionalAsync(veiculoId, opcionalId);

            if (!vinculado)
            {
                return NotFound("Veículo ou opcional não encontrado.");
            }

            return NoContent();
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE api/veiculos/5/opcionais/2 -> tira o opcional 2 do carro 5
    [HttpDelete("{veiculoId}/opcionais/{opcionalId}")]
    public async Task<IActionResult> DesvincularOpcional(int veiculoId, int opcionalId)
    {
        var desvinculado = await _service.DesvincularOpcionalAsync(veiculoId, opcionalId);

        if (!desvinculado)
        {
            return NotFound("Veículo não encontrado ou opcional não vinculado a ele.");
        }

        return NoContent();
    }
}
