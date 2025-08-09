using System;

namespace HireSphere.Core.Models;

public class Company
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Website { get; set; }
    public string LogoUrl { get; set; }
    public string Location { get; set; }
    public DateTime CreatedAt { get; set; }

    public Company(Guid id, Guid ownerUserId, string name, string description, string website, string logoUrl, string location, DateTime createdAt)
    {
        Id = id;
        OwnerUserId = ownerUserId;
        Name = name;
        Description = description;
        Website = website;
        LogoUrl = logoUrl;
        Location = location;
        CreatedAt = createdAt;
    }
}
