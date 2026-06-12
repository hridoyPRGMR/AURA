namespace Core.Models
{
    public class User : AuditedEntity
    {
        public string Email {get; private set;} = default!;
        public string Name {get; private set;} = default!;

        private User(){}

        internal User(long id, string email, string name) : base(id)
        {
            Email = email;
            Name = name;
        }
    }
}