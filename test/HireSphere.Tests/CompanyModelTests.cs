using System;
using HireSphere.Core.Models;
using FluentAssertions;
using Xunit;

namespace HireSphere.Tests
{
    public class CompanyModelTests
    {
        private const string ValidName = "Tech Innovations";
        private const string ValidDescription = "Innovative technology solutions";
        private const string ValidWebsite = "https://techinnovations.com";
        private const string ValidLogoUrl = "https://techinnovations.com/logo.png";
        private const string ValidLocation = "San Francisco, CA";

        [Fact]
        public void DefaultConstructor_SetsDefaultValues()
        {
            var company = new Company();

            company.Name.Should().BeEmpty();
            company.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            company.Description.Should().BeNull();
            company.Website.Should().BeNull();
            company.LogoUrl.Should().BeNull();
            company.Location.Should().BeNull();
        }

        [Fact]
        public void ParameterizedConstructor_SetsCorrectValues()
        {
            var companyId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();

            var company = new Company(
                companyId, 
                ownerUserId, 
                ValidName, 
                ValidDescription, 
                ValidWebsite, 
                ValidLogoUrl, 
                ValidLocation
            );

            company.Id.Should().Be(companyId);
            company.OwnerUserId.Should().Be(ownerUserId);
            company.Name.Should().Be(ValidName);
            company.Description.Should().Be(ValidDescription);
            company.Website.Should().Be(ValidWebsite);
            company.LogoUrl.Should().Be(ValidLogoUrl);
            company.Location.Should().Be(ValidLocation);
            company.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("A")]
        [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void Name_InvalidLength_ThrowsValidationException(string invalidName)
        {
            Action act = () => new Company
            {
                Id = Guid.NewGuid(),
                OwnerUserId = Guid.NewGuid(),
                Name = invalidName
            }.ValidateCompany();

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Company name must be between 2 and 100 characters*");
        }

        [Theory]
        [InlineData("AB")]
        [InlineData("Tech Innovations")]
        [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void Name_ValidLength_ShouldNotThrow(string validName)
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                OwnerUserId = Guid.NewGuid(),
                Name = validName
            };

            company.Name.Should().Be(validName);
        }

        [Theory]
        [InlineData("https://example.com", "https://example.com")]
        [InlineData("http://subdomain.example.co.uk/path", "http://subdomain.example.co.uk/path")]
        [InlineData("example.com", "http://example.com")]
        public void Website_ValidUrl_ShouldBeSet(string inputUrl, string expectedUrl)
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                OwnerUserId = Guid.NewGuid(),
                Name = ValidName,
                Website = inputUrl
            };

            company.ValidateCompany();

            company.Website.Should().Be(expectedUrl);
        }

        [Theory]
        [InlineData("invalid url")]
        [InlineData("http://")]
        [InlineData("www.")]
        public void Website_InvalidUrl_ShouldThrowValidationException(string invalidUrl)
        {
            Action act = () => new Company
            {
                Id = Guid.NewGuid(),
                OwnerUserId = Guid.NewGuid(),
                Name = ValidName,
                Website = invalidUrl
            }.ValidateCompany();

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Invalid website URL format*");
        }

        [Fact]
        public void UpdateLogoUrl_ValidUrl_ShouldUpdateLogoUrl()
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                OwnerUserId = Guid.NewGuid(),
                Name = ValidName
            };
            var newLogoUrl = "https://example.com/logo.png";

            company.UpdateLogoUrl(newLogoUrl);

            company.LogoUrl.Should().Be(newLogoUrl);
        }

        [Theory]
        [InlineData("invalid url")]
        [InlineData("http://")]
        public void UpdateLogoUrl_InvalidUrl_ShouldThrowException(string invalidUrl)
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                OwnerUserId = Guid.NewGuid(),
                Name = ValidName
            };

            Action act = () => company.UpdateLogoUrl(invalidUrl);
            act.Should().Throw<ArgumentException>()
               .WithMessage("Invalid logo URL format");
        }

        [Fact]
        public void UpdateLogoUrl_NullUrl_ShouldBeAllowed()
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                OwnerUserId = Guid.NewGuid(),
                Name = ValidName,
                LogoUrl = "https://example.com/logo.png"
            };

            company.UpdateLogoUrl(null);

            company.LogoUrl.Should().BeNull();
        }

        // [Theory]
        // [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        // public void Description_TooLong_ShouldThrowValidationException(string longDescription)
        // {
        //     Action act = () => new Company
        //     {
        //         Id = Guid.NewGuid(),
        //         OwnerUserId = Guid.NewGuid(),
        //         Name = ValidName,
        //         Description = longDescription
        //     }.ValidateCompany();

        //     act.Should().Throw<ArgumentException>()
        //        .WithMessage("*Description cannot exceed 500 characters*");
        // }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void Description_ValidLength_ShouldBeSet(string? description)
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                OwnerUserId = Guid.NewGuid(),
                Name = ValidName,
                Description = description
            };

            company.Description.Should().Be(description);
        }
    }
}
