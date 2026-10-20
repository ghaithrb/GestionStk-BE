using DAL.Entities;
using DAL.Repositories.IRepo;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private const int MaxNomLength = 50;
    private const int MaxAdresseLength = 50;
    private const int MaxEmailLength = 50;
    private const int MaxTelLength = 20;

    private readonly IClientRepository _clientRepository;

    public ClientController(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Client>>> GetAll()
    {
        return Ok(await _clientRepository.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Client>> GetById(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        return client is null ? NotFound() : Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> Create([FromBody] Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Nom))
        {
            ModelState.AddModelError(nameof(Client.Nom), "Nom est obligatoire.");
        }
        else if (client.Nom.Length > MaxNomLength)
        {
            ModelState.AddModelError(nameof(Client.Nom), $"Nom ne peut pas dépasser {MaxNomLength} caractères.");
        }

        if (string.IsNullOrWhiteSpace(client.Adresse))
        {
            ModelState.AddModelError(nameof(Client.Adresse), "Adresse est obligatoire.");
        }
        else if (client.Adresse.Length > MaxAdresseLength)
        {
            ModelState.AddModelError(nameof(Client.Adresse), $"Adresse ne peut pas dépasser {MaxAdresseLength} caractères.");
        }

        if (client.Email is { Length: > MaxEmailLength })
        {
            ModelState.AddModelError(nameof(Client.Email), $"Email ne peut pas dépasser {MaxEmailLength} caractères.");
        }

        if (client.Tel is { Length: > MaxTelLength })
        {
            ModelState.AddModelError(nameof(Client.Tel), $"Tel ne peut pas dépasser {MaxTelLength} caractères.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!string.IsNullOrWhiteSpace(client.Email) &&
            await _clientRepository.EmailExistsAsync(client.Email))
        {
            ModelState.AddModelError(nameof(Client.Email), "Cet email est déjà utilisé.");
            return ValidationProblem(ModelState);
        }

         var createdClient = await _clientRepository.AddAsync(client);
        return CreatedAtAction(nameof(GetById), new { id = createdClient.Codeclt }, createdClient);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Client client)
    {
        if (id != client.Codeclt)
        {
            return BadRequest("L'identifiant de l'URL doit correspondre à Codeclt.");
        }

        return await _clientRepository.UpdateAsync(id, client) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _clientRepository.DeleteAsync(id) ? NoContent() : NotFound();
    }
}