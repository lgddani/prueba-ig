using Kanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Persistence.Configurations;

public class TareaConfiguration : IEntityTypeConfiguration<Tarea>
{
    public void Configure(EntityTypeBuilder<Tarea> builder)
    {
        builder.ToTable("tareas");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Descripcion).HasMaxLength(2000);
        builder.Property(t => t.Prioridad).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(t => t.ColumnaId).IsRequired();
        builder.Property(t => t.Orden).IsRequired();
        builder.Property(t => t.FechaCreacion).IsRequired();

        builder.HasIndex(t => new { t.ColumnaId, t.Orden });

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(t => t.ResponsableId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
