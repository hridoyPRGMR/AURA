using Core.IServices;
using SnowflakeGenerator;

namespace Core.Models
{
    public class AuditedEntity
    {
        public long Id {get; private set;}
        public DateTime CreatedAt {get; private set;} = DateTime.UtcNow;
        public DateTime? UpdatedAt {get; protected set;}

        protected AuditedEntity(IIdGenerator generator)
        {
            Id = generator.NewId();
        }

        internal void MarkUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}