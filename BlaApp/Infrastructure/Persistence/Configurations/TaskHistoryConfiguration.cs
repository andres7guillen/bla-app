using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class TaskHistoryConfiguration
    : IEntityTypeConfiguration<TaskHistory>
{
    public void Configure(
        EntityTypeBuilder<TaskHistory> builder)
    {
        builder.ToTable("TaskHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.TaskId)
            .IsRequired();

        builder.Property(x => x.PreviousStatus)
            .HasMaxLength(50);

        builder.Property(x => x.NewStatus)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.ChangedAt)
            .IsRequired();

        builder.HasOne<TaskItem>()
            .WithMany(x => x.History)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
