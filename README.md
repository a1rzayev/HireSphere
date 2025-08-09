# HireSphere - Job Application Management Platform

## Overview

HireSphere is a comprehensive job application management platform designed to streamline the hiring process for both employers and job seekers. The platform provides robust features for job posting, application tracking, and user management.

## Project Structure

### Backend Architecture
- Clean Architecture with Separation of Concerns
- Layered solution with clear domain boundaries
- Entity Framework Core for data persistence

### Key Domains

#### 1. User Management
- Supports multiple roles: JobSeeker, Employer, Admin
- Secure authentication and authorization
- Profile management with role-based access control

#### 2. Job Management
- Create, update, and manage job listings
- Comprehensive job search and filtering
- Support for various job types and categories

#### 3. Job Application Workflow
- Advanced application status tracking
- Strict business rules for application state transitions
- Prevent duplicate applications

#### 4. Company Management
- Company profile creation and management
- Validation for company information
- Link companies with employers

## Core Technologies

- .NET Core
- Entity Framework Core
- C# 10.0
- Clean Architecture Principles

## Domain Models

### User
- Unique identification
- Role-based access
- Email and password management
- Profile information

### Job
- Detailed job listing information
- Salary range
- Job type and category
- Posting and expiration management

### Job Application
- Comprehensive application tracking
- Status transition rules
- Resume and cover letter support

### Company
- Company profile details
- Unique slug generation
- Employer association

## Business Validation Rules

### User
- Email uniqueness
- Password complexity
- Role enforcement

### Job
- Salary range validation
- Posting date constraints
- Company and category verification

### Job Application
- Strict status transition rules
- Prevent duplicate applications
- Application status management

### Company
- Unique name and slug
- Employer verification

## Getting Started

### Prerequisites
- .NET 6.0 SDK
- SQL Server or PostgreSQL
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Configure connection string in `appsettings.json`
3. Run database migrations
4. Start the application

```bash
dotnet restore
dotnet ef database update
dotnet run
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

Asif Rzayev - [Your Email or LinkedIn]

Project Link: [https://github.com/yourusername/HireSphere](https://github.com/yourusername/HireSphere)
