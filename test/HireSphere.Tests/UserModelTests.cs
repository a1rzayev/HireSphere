using System;
using HireSphere.Core.Models;
using HireSphere.Core.Enums;
using FluentAssertions;
using Xunit;
using System.Reflection;

namespace HireSphere.Tests
{
    public class UserModelTests
    {
        [Fact]
        public void DefaultConstructor_SetsDefaultValues()
        {
            var user = new User();

            user.Role.Should().Be(Role.JobSeeker);
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            user.Email.Should().BeEmpty();
            user.PasswordHash.Should().BeEmpty();
            user.Name.Should().BeEmpty();
            user.Surname.Should().BeEmpty();
        }

        [Fact]
        public void ParameterizedConstructor_SetsCorrectValues()
        {
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var passwordHash = "hashedpassword";
            var role = Role.Employer;
            var name = "John";
            var surname = "Doe";
            var phone = "+1234567890";

            var user = new User(userId, email, passwordHash, role, name, surname, phone);

            user.Id.Should().Be(userId);
            user.Email.Should().Be(email);
            user.PasswordHash.Should().Be(passwordHash);
            user.Role.Should().Be(role);
            user.Name.Should().Be(name);
            user.Surname.Should().Be(surname);
            user.Phone.Should().Be(phone);
        }

        [Fact]
        public void Constructor_InvalidRole_ThrowsArgumentException()
        {
            var invalidRole = (Role)999;

            Assert.Throws<ArgumentException>(() => new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = invalidRole,
                Name = "John",
                Surname = "Doe"
            }.ValidateUser());
        }

        [Fact]
        public void Constructor_InvalidPhoneNumber_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = Role.JobSeeker,
                Name = "John",
                Surname = "Doe",
                Phone = "invalid phone"
            }.ValidateUser());
        }

        [Theory]
        [InlineData("+1 (123) 456-7890")]
        [InlineData("123-456-7890")]
        [InlineData("+44 20 1234 5678")]
        public void IsValidPhoneNumber_ValidNumbers_ReturnsTrue(string phoneNumber)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = Role.JobSeeker,
                Name = "John",
                Surname = "Doe",
                Phone = phoneNumber
            };

            user.ValidateUser();

            user.Phone.Should().Be(phoneNumber);
        }

        [Fact]
        public void UpdateEmail_ValidEmail_UpdatesEmailAndResetsConfirmation()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "old@example.com",
                PasswordHash = "hashedpassword",
                Role = Role.JobSeeker,
                Name = "John",
                Surname = "Doe"
            };
            
            var isEmailConfirmedProperty = typeof(User).GetProperty("IsEmailConfirmed", BindingFlags.NonPublic | BindingFlags.Instance);
            isEmailConfirmedProperty?.SetValue(user, true);

            user.UpdateEmail("new@example.com");

            user.Email.Should().Be("new@example.com");
            user.IsEmailConfirmed.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("a")]
        [InlineData("a@b.c")]
        [InlineData("verylongemailthatexceedsahundredcharactersandisnotvalidaccordingtothestringlengthrequirementinthemodel@example.com")]
        public void UpdateEmail_InvalidEmail_ThrowsArgumentException(string invalidEmail)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = Role.JobSeeker,
                Name = "John",
                Surname = "Doe"
            };

            Assert.Throws<ArgumentException>(() => user.UpdateEmail(invalidEmail));
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("invalid@")]
        [InlineData("@invalid.com")]
        [InlineData("invalid@invalid")]
        public void UpdateEmail_InvalidEmailFormat_ThrowsArgumentException(string invalidEmail)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = Role.JobSeeker,
                Name = "John",
                Surname = "Doe"
            };

            Assert.Throws<ArgumentException>(() => user.UpdateEmail(invalidEmail));
        }
    }
}
