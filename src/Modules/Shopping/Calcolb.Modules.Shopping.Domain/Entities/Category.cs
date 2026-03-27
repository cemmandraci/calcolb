using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Shopping.Domain.Entities;

public sealed class Category : Entity<Guid>
{
    public string Name { get; private set; } = null!;
    public Guid? ParentCategoryId { get; private set; }

    private Category() { }

    public static Category Create(string name, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Kategori adı zorunludur.");

        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            ParentCategoryId = parentCategoryId
        };
    }
}
