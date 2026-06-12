using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TaskStepConfiguration
    : IEntityTypeConfiguration<TaskStep>
    {
        public void Configure(EntityTypeBuilder<TaskStep> builder)
        {
            builder.ToTable("task_steps");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Instruction)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.Result);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasOne(x => x.TaskItem)
                .WithMany(x => x.Steps)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.TaskItemId, x.Order });
        }
    }
}