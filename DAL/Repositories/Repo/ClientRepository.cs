using DAL.Entities;
using DAL.Repositories.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Repo;

public class ClientRepository : IClientRepository
{
    private readonly STKDbContext _context;

    public ClientRepository(STKDbContext context)
    {
        _context = context;
    }

    public Task<List<Client>> GetAllAsync() => _context.Clients.AsNoTracking().ToListAsync();

    public Task<Client?> GetByIdAsync(int id) =>
        _context.Clients.AsNoTracking().FirstOrDefaultAsync(client => client.Codeclt == id);

    public Task<bool> EmailExistsAsync(string email) =>
        _context.Clients.AnyAsync(client => client.Email == email);

    public async Task<Client> AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<bool> UpdateAsync(int id, Client client)
    {
        var existingClient = await _context.Clients.FindAsync(id);
        if (existingClient is null)
        {
            return false;
        }

        existingClient.Nom = client.Nom;
        existingClient.Adresse = client.Adresse;
        existingClient.Email = client.Email;
        existingClient.Tel = client.Tel;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null)
        {
            return false;
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
}