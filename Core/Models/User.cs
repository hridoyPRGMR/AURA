using Core.Models;
using Core.IServices;

namespace Core.Models
{
    public class User : AuditedEntity
    {
        public string Email {get; private set;} = default!;
        public string Name {get; private set;} = default!;

        internal User(IIdGenerator generator, string email, string name) : base(generator)
        {
            Email = email;
            Name = name;
        }
    }
}