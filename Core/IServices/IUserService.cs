using Shared.Dtos;

namespace Core.IServices
{
    public interface IUserService
    {
        Task<UserDto> GetUser();
    }
}