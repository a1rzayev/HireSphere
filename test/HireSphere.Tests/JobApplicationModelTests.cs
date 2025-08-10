using System;
using HireSphere.Core.Models;
using HireSphere.Core.Enums;
using FluentAssertions;
using Xunit;

namespace HireSphere.Tests
{
    public class JobApplicationModelTests
    {
        private const string ValidResumeUrl = "https://example.com/resume.pdf";
        private const string ValidCoverLetter = "I am excited about the opportunity to join your team...";

        [Fact]
        public void DefaultConstructor_SetsDefaultValues()
        {
            var jobApplication = new JobApplication();

            jobApplication.ResumeUrl.Should().BeEmpty();
            jobApplication.Status.Should().Be(JobApplicationStatus.Applied);
            jobApplication.AppliedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            jobApplication.CoverLetter.Should().BeNull();
        }

        [Fact]
        public void ParameterizedConstructor_SetsCorrectValues()
        {
            var applicationId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var applicantUserId = Guid.NewGuid();
            var appliedAt = DateTime.UtcNow.AddDays(-1);

            var jobApplication = new JobApplication(
                applicationId, 
                jobId, 
                applicantUserId, 
                ValidResumeUrl, 
                ValidCoverLetter, 
                JobApplicationStatus.Screening, 
                appliedAt
            );

            jobApplication.Id.Should().Be(applicationId);
            jobApplication.JobId.Should().Be(jobId);
            jobApplication.ApplicantUserId.Should().Be(applicantUserId);
            jobApplication.ResumeUrl.Should().Be(ValidResumeUrl);
            jobApplication.CoverLetter.Should().Be(ValidCoverLetter);
            jobApplication.Status.Should().Be(JobApplicationStatus.Screening);
            jobApplication.AppliedAt.Should().Be(appliedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("not-a-url")]
        public void InvalidResumeUrl_ShouldThrowArgumentException(string invalidResumeUrl)
        {
            Action act = () => new JobApplication
            {
                Id = Guid.NewGuid(),
                JobId = Guid.NewGuid(),
                ApplicantUserId = Guid.NewGuid(),
                ResumeUrl = invalidResumeUrl
            }.ValidateApplication();

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Invalid resume URL format.*");
        }

        [Fact]
        public void InvalidStatus_ShouldThrowArgumentException()
        {
            Action act = () => new JobApplication
            {
                Id = Guid.NewGuid(),
                JobId = Guid.NewGuid(),
                ApplicantUserId = Guid.NewGuid(),
                ResumeUrl = ValidResumeUrl,
                Status = (JobApplicationStatus)999
            }.ValidateApplication();

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Invalid application status.*");
        }

        [Theory]
        [InlineData(JobApplicationStatus.Applied, JobApplicationStatus.Screening)]
        [InlineData(JobApplicationStatus.Applied, JobApplicationStatus.Rejected)]
        [InlineData(JobApplicationStatus.Screening, JobApplicationStatus.Interview)]
        [InlineData(JobApplicationStatus.Screening, JobApplicationStatus.Rejected)]
        [InlineData(JobApplicationStatus.Interview, JobApplicationStatus.Offered)]
        [InlineData(JobApplicationStatus.Interview, JobApplicationStatus.Rejected)]
        [InlineData(JobApplicationStatus.Offered, JobApplicationStatus.Accepted)]
        [InlineData(JobApplicationStatus.Offered, JobApplicationStatus.Rejected)]
        public void ValidStatusTransition_ShouldSucceed(JobApplicationStatus currentStatus, JobApplicationStatus newStatus)
        {
            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobId = Guid.NewGuid(),
                ApplicantUserId = Guid.NewGuid(),
                ResumeUrl = ValidResumeUrl,
                Status = currentStatus
            };

            Action act = () => jobApplication.ChangeStatus(newStatus);

            act.Should().NotThrow();
            jobApplication.Status.Should().Be(newStatus);
        }

        [Theory]
        [InlineData(JobApplicationStatus.Applied, JobApplicationStatus.Offered)]
        [InlineData(JobApplicationStatus.Screening, JobApplicationStatus.Offered)]
        [InlineData(JobApplicationStatus.Interview, JobApplicationStatus.Screening)]
        [InlineData(JobApplicationStatus.Offered, JobApplicationStatus.Interview)]
        [InlineData(JobApplicationStatus.Rejected, JobApplicationStatus.Applied)]
        [InlineData(JobApplicationStatus.Accepted, JobApplicationStatus.Interview)]
        [InlineData(JobApplicationStatus.Withdrawn, JobApplicationStatus.Applied)]
        public void InvalidStatusTransition_ShouldThrowInvalidOperationException(JobApplicationStatus currentStatus, JobApplicationStatus newStatus)
        {
            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobId = Guid.NewGuid(),
                ApplicantUserId = Guid.NewGuid(),
                ResumeUrl = ValidResumeUrl,
                Status = currentStatus
            };

            Action act = () => jobApplication.ChangeStatus(newStatus);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage($"*Invalid status transition from {currentStatus}.*");
        }

        [Theory]
        [InlineData(JobApplicationStatus.Rejected)]
        [InlineData(JobApplicationStatus.Accepted)]
        [InlineData(JobApplicationStatus.Withdrawn)]
        public void FinalStatusTransition_ShouldThrowInvalidOperationException(JobApplicationStatus finalStatus)
        {
            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobId = Guid.NewGuid(),
                ApplicantUserId = Guid.NewGuid(),
                ResumeUrl = ValidResumeUrl,
                Status = finalStatus
            };

            Action act = () => jobApplication.ChangeStatus(JobApplicationStatus.Applied);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage($"*Invalid status transition from {finalStatus}.*");
        }

        [Fact]
        public void AddCoverLetter_ValidCoverLetter_ShouldUpdateCoverLetter()
        {
            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobId = Guid.NewGuid(),
                ApplicantUserId = Guid.NewGuid(),
                ResumeUrl = ValidResumeUrl
            };

            jobApplication.AddCoverLetter(ValidCoverLetter);

            jobApplication.CoverLetter.Should().Be(ValidCoverLetter);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void AddCoverLetter_EmptyCoverLetter_ShouldThrowArgumentException(string invalidCoverLetter)
        {
            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobId = Guid.NewGuid(),
                ApplicantUserId = Guid.NewGuid(),
                ResumeUrl = ValidResumeUrl
            };

            Action act = () => jobApplication.AddCoverLetter(invalidCoverLetter);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Cover letter cannot be empty.*");
        }

        [Fact]
        public void AddCoverLetter_TooLongCoverLetter_ShouldThrowArgumentException()
        {
            var jobApplication = new JobApplication
            {
                Id = Guid.NewGuid(),
                JobId = Guid.NewGuid(),
                ApplicantUserId = Guid.NewGuid(),
                ResumeUrl = ValidResumeUrl
            };

            Action act = () => jobApplication.AddCoverLetter(new string('A', 2001));

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Cover letter cannot exceed 2000 characters.*");
        }
    }
}
