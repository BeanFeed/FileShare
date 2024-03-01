using DAL.Entities;

namespace FileshareBackend.Services.Interfaces;

public interface IJwtService
{
    public string EncodeToken(User user);
    public Task<User> DecodeToken(string eToken);
}