namespace Shared.Dtos
{
    public class AuditedDto
    {
        public long Id {get; init;}
        public DateTime CreatedAt {get; init;}
        public DateTime? UpdatedAt {get; init;}
    }
    
}