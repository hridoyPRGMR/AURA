using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("tasks");

            builder.HasKey(x=> x.Id);

            builder.Property(x=> x.Id)
                .ValueGeneratedNever();

            builder.Property(x=> x.UserPrompt)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x=> x.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasMany(x => x.Steps)
                .WithOne(x => x.TaskItem)
                .HasForeignKey(x => x.TaskItemId);
        }
    }
}