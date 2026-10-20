using DAL.Entities;

namespace DAL.Repositories.IRepo;

public interface IClientRepository
{
    Task<List<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task<Client> AddAsync(Client client);
    Task<bool> UpdateAsync(int id, Client client);
    Task<bool> DeleteAsync(int id);
}