# HireSphere

HireSphere is a modern job searching portal designed to connect job seekers, employers, and companies in a seamless and efficient way. The platform supports job postings, applications, company profiles, and job categorization, with a clean separation of concerns and scalable architecture.

## Core Features
- User authentication and role management (Admin, Employer, JobSeeker)
- Company profiles managed by employers
- Job postings with rich metadata (salary, type, location, etc.)
- Job applications with workflow states
- Job categorization for easy filtering and discovery

## Domain-Driven Architecture
The backend is structured using Domain-Driven Design (DDD) principles, with clear separation between models, enums, and repository interfaces. Business logic is implemented in service and repository layers, keeping models clean and focused.

### Domain Models
| Model          | Main Focus            | Key Properties / Relations                                  |
| -------------- | --------------------- | ----------------------------------------------------------- |
| User           | Authentication, roles | Id, Email, PasswordHash, Role, Name, Surname, Phone, etc.   |
| Company        | Employer companies    | Id, OwnerUserId (User), Name, Description, etc.             |
| Job            | Job postings          | Id, CompanyId, CategoryId, Title, Salary, JobType, etc.     |
| JobApplication | Job applications      | Id, JobId, ApplicantUserId, ResumeUrl, Status, AppliedAt    |
| Category       | Job classification    | Id, Name, Slug                                              |

### Relationships
- **User** can own multiple **Companies** (via OwnerUserId)
- **Company** can post multiple **Jobs** (via CompanyId)
- **Job** belongs to a **Category** (via CategoryId)
- **JobApplication** links a **User** (Applicant) to a **Job**

### Enums
- `Role`: Admin, Employer, JobSeeker
- `JobType`: FullTime, PartTime, Contract, Internship, Temporary
- `JobApplicationStatus`: Applied, Viewed, Interviewing, Offered, Rejected

### Repository Interfaces
Each model has a corresponding repository interface for CRUD operations, e.g.:
- `IUserRepository`
- `ICompanyRepository`
- `IJobRepository`
- `IJobApplicationRepository`
- `ICategoryRepository`

## Business Logic
Business rules (such as validation, workflow, and unique constraints) are implemented in service and repository layers, not in the models themselves. This ensures maintainability and testability.

## Project Structure
```
api/HireSphere.Core/
  Models/         # Domain models (User, Company, Job, etc.)
  Enums/          # Enum types (Role, JobType, etc.)
  ...
api/HireSphere.Infrastructure/
  # Repository implementations, data access, etc.
front/HideSphere.Presentation/
  # Web frontend (MVC, Razor, etc.)
```

## Getting Started
1. Clone the repository
2. Build the solution
3. Configure your database and environment variables
4. Run the backend and frontend projects

---

For more details, see the code in the `api/HireSphere.Core/Models` and `api/HireSphere.Core/Enums` folders.
