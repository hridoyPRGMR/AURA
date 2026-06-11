namespace Shared.Dtos
{
    public class UserDto : AuditedDto
    {
        public string Email {get; init;} = default!;
        public string Name {get; init;} = default!;
    }
}