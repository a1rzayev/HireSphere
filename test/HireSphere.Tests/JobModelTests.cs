using System;
using System.Collections.Generic;
using HireSphere.Core.Models;
using HireSphere.Core.Enums;
using FluentAssertions;
using Xunit;

namespace HireSphere.Tests
{
    public class JobModelTests
    {
        private const string ValidTitle = "Senior Software Engineer";
        private const string ValidDescription = "We are seeking a talented software engineer with 5+ years of experience in backend development.";
        private const string ValidRequirements = "- 5+ years of experience\n- Proficiency in C# and .NET\n- Strong understanding of software design principles";
        private const string ValidLocation = "San Francisco, CA";
        private const decimal ValidSalaryFrom = 100000m;
        private const decimal ValidSalaryTo = 150000m;

        [Fact]
        public void DefaultConstructor_SetsDefaultValues()
        {
            var job = new Job();

            job.Title.Should().BeEmpty();
            job.Description.Should().BeEmpty();
            job.PostedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            job.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(30), TimeSpan.FromSeconds(1));
            job.IsActive.Should().BeTrue();
        }

        [Fact]
        public void ParameterizedConstructor_SetsCorrectValues()
        {
            var jobId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var tags = new List<string> { "backend", "dotnet" };
            var postedAt = DateTime.UtcNow.AddDays(-1);
            var expiresAt = postedAt.AddDays(45);

            var job = new Job(
                jobId, 
                companyId, 
                ValidTitle, 
                ValidDescription, 
                ValidRequirements, 
                ValidSalaryFrom, 
                ValidSalaryTo, 
                ValidLocation, 
                JobType.FullTime, 
                true, 
                categoryId, 
                tags, 
                postedAt, 
                expiresAt, 
                true
            );

            job.Id.Should().Be(jobId);
            job.CompanyId.Should().Be(companyId);
            job.Title.Should().Be(ValidTitle);
            job.Description.Should().Be(ValidDescription);
            job.Requirements.Should().Be(ValidRequirements);
            job.SalaryFrom.Should().Be(ValidSalaryFrom);
            job.SalaryTo.Should().Be(ValidSalaryTo);
            job.Location.Should().Be(ValidLocation);
            job.JobType.Should().Be(JobType.FullTime);
            job.IsRemote.Should().BeTrue();
            job.CategoryId.Should().Be(categoryId);
            job.Tags.Should().BeEquivalentTo(tags);
            job.PostedAt.Should().Be(postedAt);
            job.ExpiresAt.Should().Be(expiresAt);
            job.IsActive.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Description")]
        [InlineData(" ", "Description")]
        [InlineData(null, "Description")]
        [InlineData("A", "Title")]
        [InlineData(" ", "Title")]
        public void InvalidTitleOrDescription_ShouldThrowArgumentException(string invalidValue, string propertyName)
        {
            Action act = () => new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = propertyName == "Title" ? invalidValue : ValidTitle,
                Description = propertyName == "Description" ? invalidValue : ValidDescription,
                JobType = JobType.FullTime,
                CategoryId = Guid.NewGuid()
            }.ValidateJob();

            act.Should().Throw<ArgumentException>()
               .WithMessage($"*{propertyName} is required and must be at least {(propertyName == "Title" ? "2" : "10")} characters long.*");
        }

        [Fact]
        public void SalaryFrom_GreaterThan_SalaryTo_ShouldThrowArgumentException()
        {
            Action act = () => new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = ValidTitle,
                Description = ValidDescription,
                JobType = JobType.FullTime,
                CategoryId = Guid.NewGuid(),
                SalaryFrom = 150000m,
                SalaryTo = 100000m
            }.ValidateJob();

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Salary 'from' cannot be greater than salary 'to'.*");
        }

        [Fact]
        public void ExpiresAt_BeforePostedAt_ShouldThrowArgumentException()
        {
            var postedAt = DateTime.UtcNow;
            var expiresAt = postedAt.AddDays(-1);

            Action act = () => new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = ValidTitle,
                Description = ValidDescription,
                JobType = JobType.FullTime,
                CategoryId = Guid.NewGuid(),
                PostedAt = postedAt,
                ExpiresAt = expiresAt
            }.ValidateJob();

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Job expiration date must be after the posting date.*");
        }

        [Fact]
        public void InvalidJobType_ShouldThrowArgumentException()
        {
            Action act = () => new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = ValidTitle,
                Description = ValidDescription,
                JobType = (JobType)999,
                CategoryId = Guid.NewGuid()
            }.ValidateJob();

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Invalid job type.*");
        }

        [Fact]
        public void Activate_NonExpiredJob_ShouldSetIsActiveToTrue()
        {
            var job = new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = ValidTitle,
                Description = ValidDescription,
                JobType = JobType.FullTime,
                CategoryId = Guid.NewGuid(),
                IsActive = false,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            job.Activate();

            job.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Activate_ExpiredJob_ShouldThrowInvalidOperationException()
        {
            var job = new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = ValidTitle,
                Description = ValidDescription,
                JobType = JobType.FullTime,
                CategoryId = Guid.NewGuid(),
                IsActive = false,
                ExpiresAt = DateTime.UtcNow.AddDays(-1)
            };

            Action act = () => job.Activate();

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*Cannot activate an expired job.*");
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            var job = new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = ValidTitle,
                Description = ValidDescription,
                JobType = JobType.FullTime,
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };

            job.Deactivate();

            job.IsActive.Should().BeFalse();
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(-1, true)]
        public void IsExpired_ShouldReturnCorrectValue(int daysOffset, bool expectedIsExpired)
        {
            var job = new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = ValidTitle,
                Description = ValidDescription,
                JobType = JobType.FullTime,
                CategoryId = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddDays(daysOffset)
            };

            var isExpired = job.IsExpired();

            isExpired.Should().Be(expectedIsExpired);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(60)]
        public void ExtendExpiration_ValidDays_ShouldExtendExpirationDate(int extensionDays)
        {
            var originalExpiresAt = DateTime.UtcNow.AddDays(30);
            var job = new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = ValidTitle,
                Description = ValidDescription,
                JobType = JobType.FullTime,
                CategoryId = Guid.NewGuid(),
                ExpiresAt = originalExpiresAt
            };

            job.ExtendExpiration(extensionDays);

            job.ExpiresAt.Should().Be(originalExpiresAt.AddDays(extensionDays));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ExtendExpiration_InvalidDays_ShouldThrowArgumentException(int invalidDays)
        {
            var job = new Job
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Title = ValidTitle,
                Description = ValidDescription,
                JobType = JobType.FullTime,
                CategoryId = Guid.NewGuid()
            };

            Action act = () => job.ExtendExpiration(invalidDays);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Extension days must be positive.*");
        }
    }
}
