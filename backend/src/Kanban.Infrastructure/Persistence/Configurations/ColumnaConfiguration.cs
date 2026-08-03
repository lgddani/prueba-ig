using Kanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Persistence.Configurations;

public class ColumnaConfiguration : IEntityTypeConfiguration<Columna>
{
    public void Configure(EntityTypeBuilder<Columna> builder)
    {
        builder.ToTable("columnas");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Orden).IsRequired();
        builder.Property(c => c.ProyectoId).IsRequired();

        builder.HasIndex(c => new { c.ProyectoId, c.Orden });

        builder.HasMany(c => c.Tareas)
            .WithOne()
            .HasForeignKey(t => t.ColumnaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Metadata.FindNavigation(nameof(Columna.Tareas))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
