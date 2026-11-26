using Microsoft.AspNetCore.Mvc;
using ApexDrive.Models;
using ApexDrive.Services;

namespace ApexDrive.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OpcionaisController : ControllerBase
	{
		private readonly IOpcionaisService _service;

		public OpcionaisController(IOpcionaisService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Opcional>>> GetOpcional()
		{
			return Ok(await _service.ListarAsync());
		}

		[HttpPost]
		public async Task<ActionResult<Opcional>> PostOpcional(Opcional opcional)
		{
			try
			{
				var criado = await _service.CriarAsync(opcional);
				return CreatedAtAction(nameof(GetOpcional), new { id = criado.Id }, criado);
			}
			catch (RegraDeNegocioException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutOpcional(int id, Opcional opcionalAtualizado)
		{
			try
			{
				var atualizado = await _service.AtualizarAsync(id, opcionalAtualizado);

				if (!atualizado)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (RegraDeNegocioException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteOpcional(int id)
		{
			var removido = await _service.RemoverAsync(id);

			if (!removido)
			{
				return NotFound();
			}

			return NoContent();
		}
	}
}
