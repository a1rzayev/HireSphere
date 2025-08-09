using System;

namespace HireSphere.Core.Models;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }

    public Category(Guid id, string name, string slug)
    {
        Id = id;
        Name = name;
        Slug = slug;
    }
}
