namespace Infrastructure.Persistence.Configurations
{
    using Core.Models;
    using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TaskResultConfiguration
    : IEntityTypeConfiguration<TaskResult>
{
    public void Configure(EntityTypeBuilder<TaskResult> builder)
    {
        builder.ToTable("task_results");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FinalOutput)
            .IsRequired();

        builder.Property(x => x.Summary)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Task)
            .WithOne(x => x.Result)
            .HasForeignKey<TaskResult>(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TaskId)
            .IsUnique();
    }
}
}