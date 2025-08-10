using System;
using System.Text.RegularExpressions;
using HireSphere.Core.Models;
using FluentAssertions;
using Xunit;

namespace HireSphere.Tests
{
    public class CategoryModelTests
    {
        private const string ValidCategoryName = "Software Engineering";
        private const string ValidCategoryName2 = "Data Science";

        [Fact]
        public void DefaultConstructor_SetsDefaultValues()
        {
            var category = new Category();

            category.Name.Should().BeEmpty();
            category.Slug.Should().BeEmpty();
        }

        [Fact]
        public void ParameterizedConstructor_SetsCorrectValues()
        {
            var categoryId = Guid.NewGuid();

            var category = new Category(categoryId, ValidCategoryName);

            category.Id.Should().Be(categoryId);
            category.Name.Should().Be(ValidCategoryName);
            category.Slug.Should().Be("software-engineering");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("A")]
        [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void InvalidName_ShouldThrowArgumentException(string invalidName)
        {
            Action act = () => new Category(Guid.NewGuid(), invalidName);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Category name must be between 2 and 100 characters.*");
        }

        [Theory]
        [InlineData("Invalid Category!")]
        [InlineData("Category@Test")]
        [InlineData("Category_Test")]
        public void InvalidNameFormat_ShouldThrowArgumentException(string invalidName)
        {
            Action act = () => new Category(Guid.NewGuid(), invalidName);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Category name can only contain letters, numbers, spaces, and hyphens*");
        }

        // [Theory]
        // [InlineData("Software Engineering", "software-engineering")]
        // [InlineData("Data Science", "data-science")]
        // [InlineData("Cloud & DevOps", "cloud-devops")]
        // [InlineData("Machine Learning 101", "machine-learning-101")]
        // [InlineData("  Trimmed Category  ", "trimmed-category")]
        // [InlineData("a", "category")]
        // public void GenerateSlug_ShouldCreateCorrectSlug(string inputName, string expectedSlug)
        // {
        //     var category = new Category(Guid.NewGuid(), inputName);

        //     category.Slug.Should().Be(expectedSlug);
        // }

        [Fact]
        public void UpdateName_ValidName_ShouldUpdateNameAndSlug()
        {
            var category = new Category(Guid.NewGuid(), ValidCategoryName);
            var newName = ValidCategoryName2;

            category.UpdateName(newName);

            category.Name.Should().Be(newName);
            category.Slug.Should().Be("data-science");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("A")]
        [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void UpdateName_InvalidName_ShouldThrowArgumentException(string invalidName)
        {
            var category = new Category(Guid.NewGuid(), ValidCategoryName);

            Action act = () => category.UpdateName(invalidName);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Category name must be between 2 and 100 characters.*");
        }

        [Theory]
        [InlineData("Invalid Category!")]
        [InlineData("Category@Test")]
        [InlineData("Category_Test")]
        public void UpdateName_InvalidNameFormat_ShouldThrowArgumentException(string invalidName)
        {
            var category = new Category(Guid.NewGuid(), ValidCategoryName);

            Action act = () => category.UpdateName(invalidName);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Category name can only contain letters, numbers, spaces, and hyphens*");
        }
    }
}
