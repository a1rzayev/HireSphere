# HireSphere - Job Portal API

A comprehensive job portal API built with ASP.NET Core, Entity Framework Core, and PostgreSQL. This project provides a complete backend solution for job posting, application management, and user management.

## 🚀 Features

- **User Management**: Complete CRUD operations for user accounts
- **Job Management**: Job posting and management with filtering capabilities
- **Job Applications**: Application tracking with status management
- **Company Management**: Company profiles with search and filtering
- **Category Management**: Job categories with SEO-friendly slugs
- **RESTful API**: Full REST API with proper HTTP status codes
- **Entity Framework Core**: Modern ORM with PostgreSQL
- **Swagger Documentation**: Interactive API documentation
- **Business Logic Validation**: Comprehensive validation and business rules

## 🏗️ Project Structure

```
HireSphere/
├── api/
│   ├── HireSphere.Application/          # API Controllers & Configuration
│   │   ├── Controllers/                 # REST API Controllers
│   │   │   ├── UserController.cs
│   │   │   ├── JobController.cs
│   │   │   ├── JobApplicationController.cs
│   │   │   ├── CompanyController.cs
│   │   │   └── CategoryController.cs
│   │   ├── Program.cs                   # Application configuration
│   │   └── appsettings.json            # Database connection
│   ├── HireSphere.Core/                # Domain Models & Interfaces
│   │   ├── Models/                      # Entity models
│   │   ├── Enums/                       # Enumerations
│   │   ├── DTOs/                        # Data Transfer Objects
│   │   └── Repositories/                # Repository interfaces
│   └── HireSphere.Infrastructure/      # Data Access & Services
│       ├── ORM/                         # Entity Framework context
│       ├── Repositories/                # Repository implementations
│       └── Services/                    # Business services
├── front/                               # Frontend application (MVC)
└── test/                                # Unit tests
```

## 🛠️ Technology Stack

- **.NET 8.0**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 9.0.4**: ORM for database operations
- **PostgreSQL**: Primary database
- **Swagger/OpenAPI**: API documentation
- **Npgsql**: PostgreSQL provider for EF Core

## 📋 Prerequisites

- .NET 8.0 SDK
- PostgreSQL 12+ 
- macOS/Linux/Windows

## 🔧 Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd HireSphere
```

### 2. Database Setup

#### Install PostgreSQL (macOS)
```bash
# Using Homebrew
brew install postgresql
brew services start postgresql

# Create database
createdb -U postgres hiresphere
```

#### Install PostgreSQL (Ubuntu/Debian)
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Create database
sudo -u postgres createdb hiresphere
```

### 3. Configure Database Connection

Update the connection string in `api/HireSphere.Application/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=hiresphere;Username=postgres;Password=your_password"
  }
}
```

### 4. Install Dependencies
```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build
```

### 5. Run Database Migrations
```bash
# Navigate to the Infrastructure project
cd api/HireSphere.Infrastructure

# Add initial migration
dotnet ef migrations add InitialCreate --startup-project ../HireSphere.Application

# Apply migrations to database
dotnet ef database update --startup-project ../HireSphere.Application
```

### 6. Run the Application
```bash
# Navigate to the Application project
cd api/HireSphere.Application

# Run the application
dotnet run
```

The API will be available at: `http://localhost:5005`

## 📚 API Documentation

### Swagger UI

Once the application is running, you can access the interactive API documentation at:

**http://localhost:5005/swagger**

This provides:
- Complete API endpoint documentation
- Interactive testing interface
- Request/response schemas
- Authentication requirements (if configured)

### API Endpoints Overview

#### User Management
- `GET /api/user` - Get all users
- `GET /api/user/{id}` - Get user by ID
- `POST /api/user` - Create new user
- `PUT /api/user/{id}` - Update user
- `DELETE /api/user/{id}` - Delete user

#### Job Management
- `GET /api/job` - Get all jobs
- `GET /api/job/{id}` - Get job by ID
- `POST /api/job` - Create new job
- `PUT /api/job/{id}` - Update job
- `DELETE /api/job/{id}` - Delete job
- `GET /api/job/company/{companyId}` - Get jobs by company
- `GET /api/job/category/{categoryId}` - Get jobs by category
- `GET /api/job/type/{jobType}` - Get jobs by type
- `GET /api/job/search/{title}` - Search jobs by title

#### Job Application Management
- `GET /api/jobapplication` - Get all applications
- `GET /api/jobapplication/{id}` - Get application by ID
- `POST /api/jobapplication` - Create new application
- `PUT /api/jobapplication/{id}` - Update application
- `DELETE /api/jobapplication/{id}` - Delete application
- `GET /api/jobapplication/job/{jobId}` - Get applications by job
- `GET /api/jobapplication/applicant/{applicantUserId}` - Get applications by applicant
- `GET /api/jobapplication/status/{status}` - Get applications by status
- `PUT /api/jobapplication/{id}/status` - Update application status
- `PUT /api/jobapplication/{id}/cover-letter` - Update cover letter

#### Company Management
- `GET /api/company` - Get all companies
- `GET /api/company/{id}` - Get company by ID
- `POST /api/company` - Create new company
- `PUT /api/company/{id}` - Update company
- `DELETE /api/company/{id}` - Delete company
- `GET /api/company/owner/{ownerUserId}` - Get companies by owner
- `GET /api/company/search/name/{name}` - Search companies by name
- `GET /api/company/search/location/{location}` - Search companies by location
- `PUT /api/company/{id}/logo` - Update company logo

#### Category Management
- `GET /api/category` - Get all categories
- `GET /api/category/{id}` - Get category by ID
- `POST /api/category` - Create new category
- `PUT /api/category/{id}` - Update category
- `DELETE /api/category/{id}` - Delete category
- `GET /api/category/name/{name}` - Get category by name
- `GET /api/category/slug/{slug}` - Get category by slug
- `GET /api/category/search/{name}` - Search categories by name

## 🧪 Testing the API

### Using curl

#### Create a User
```bash
curl -X POST http://localhost:5005/api/user \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John",
    "surname": "Doe",
    "email": "john.doe@example.com",
    "passwordHash": "hashedpassword123",
    "role": 0
  }'
```

#### Create a Company
```bash
curl -X POST http://localhost:5005/api/company \
  -H "Content-Type: application/json" \
  -d '{
    "ownerUserId": "00000000-0000-0000-0000-000000000001",
    "name": "TechCorp Solutions",
    "description": "A leading technology company",
    "website": "https://techcorp.com",
    "location": "San Francisco, CA"
  }'
```

#### Create a Job
```bash
curl -X POST http://localhost:5005/api/job \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "00000000-0000-0000-0000-000000000001",
    "title": "Senior Software Engineer",
    "description": "We are looking for a senior software engineer...",
    "requirements": "5+ years of experience, C#, .NET",
    "salary": 120000,
    "jobType": 0,
    "location": "San Francisco, CA"
  }'
```

### Using Swagger UI

1. Open your browser and navigate to `http://localhost:5005/swagger`
2. You'll see all available endpoints organized by controller
3. Click on any endpoint to expand it
4. Click "Try it out" to test the endpoint
5. Fill in the required parameters
6. Click "Execute" to send the request

## 🔍 Business Logic Features

### User Management
- Email validation and uniqueness
- Role-based access control (Admin, Employer, JobSeeker)
- Password hashing and security

### Job Management
- Job type categorization (FullTime, PartTime, Contract, Internship)
- Salary range validation
- Company association validation

### Job Application Management
- Status tracking (Applied, UnderReview, Interviewed, Offered, Rejected, Withdrawn)
- Business logic for status transitions
- Cover letter management
- Resume URL validation

### Company Management
- Website URL validation and formatting
- Logo URL validation
- Owner user association
- Location-based search

### Category Management
- Automatic slug generation for SEO
- Name validation and uniqueness
- Character restrictions for names

## 🧪 Running Tests

```bash
# Navigate to test project
cd test/HireSphere.Tests

# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📦 Deployment

### Docker (Optional)

Create a `Dockerfile` in the root directory:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["api/HireSphere.Application/HireSphere.Application.csproj", "api/HireSphere.Application/"]
COPY ["api/HireSphere.Core/HireSphere.Core.csproj", "api/HireSphere.Core/"]
COPY ["api/HireSphere.Infrastructure/HireSphere.Infrastructure.csproj", "api/HireSphere.Infrastructure/"]
RUN dotnet restore "api/HireSphere.Application/HireSphere.Application.csproj"
COPY . .
WORKDIR "/src/api/HireSphere.Application"
RUN dotnet build "HireSphere.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HireSphere.Application.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HireSphere.Application.dll"]
```

Build and run with Docker:
```bash
docker build -t hiresphere-api .
docker run -p 5005:5005 hiresphere-api
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

If you encounter any issues or have questions:

1. Check the [Issues](https://github.com/your-repo/issues) page
2. Create a new issue with detailed information
3. Contact the development team

## 🔄 Version History

- **v1.0.0** - Initial release with complete CRUD operations
- Complete API documentation with Swagger
- Entity Framework Core integration
- PostgreSQL database support
- Comprehensive business logic validation

---

**Happy Coding! 🚀**
