using Kanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanban.Infrastructure.Persistence.Configurations;

public class ProyectoConfiguration : IEntityTypeConfiguration<Proyecto>
{
    public void Configure(EntityTypeBuilder<Proyecto> builder)
    {
        builder.ToTable("proyectos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Descripcion).HasMaxLength(2000);
        builder.Property(p => p.FechaInicio).IsRequired();
        builder.Property(p => p.FechaFinPrevista).IsRequired();
        builder.Property(p => p.Estado).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(p => p.FechaCreacion).IsRequired();

        builder.HasIndex(p => p.Nombre);

        builder.HasMany(p => p.Columnas)
            .WithOne()
            .HasForeignKey(c => c.ProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Proyecto.Columnas))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
