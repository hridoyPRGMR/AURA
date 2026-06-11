using Core.IServices;
using Core.Models;
using Shared.Dtos;

namespace Core.Services
{
    public class UserService(
        IIdGenerator generator
    ) : IUserService
    {

        readonly User user = new(generator, "hridoy@gmail.com","Hridoy"); 

        public Task<UserDto> GetUser()
        {
            return Task.FromResult(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                CreatedAt = user.CreatedAt
            });
        }
    }
}