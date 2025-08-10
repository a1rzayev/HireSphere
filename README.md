# HireSphere

HireSphere is a comprehensive job application and recruitment management platform designed to streamline the hiring process for both employers and job seekers.

## 🌟 Project Overview

HireSphere provides a robust solution for managing job postings, applications, and the entire recruitment workflow with a focus on clean architecture and domain-driven design.

## 🏗️ Project Structure

```
HireSphere/
├── api/
│   ├── HireSphere.Core/           # Domain models and core business logic
│   │   ├── Enums/                 # Enum definitions
│   │   ├── Models/                # Domain entities
│   │   ├── Repositories/          # Repository interfaces
│   │   └── Filters/               # Search and filtering models
│   │
│   ├── HireSphere.Infrastructure/ # Data access and external services
│   │   ├── ORM/                   # Database context
│   │   ├── Repositories/          # Repository implementations
│   │   └── Services/              # Infrastructure services
│   │
│   └── HireSphere.Application/    # Application layer and entry point
│
├── front/
│   └── HideSphere.Presentation/   # Web application and controllers
│
└── test/
    └── HireSphere.Tests/          # Unit and integration tests
```

## 🚀 Features

- 📋 Job Posting Management
- 👥 User Authentication and Authorization
- 💼 Company and Job Category Management
- 📝 Job Application Tracking
- 🔍 Advanced Search and Filtering

## 🛠️ Technologies

- Backend: .NET 8
- Frontend: ASP.NET Core MVC
- Testing: xUnit, FluentAssertions
- Architecture: Domain-Driven Design (DDD)

## 📦 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

## 🔧 Setup and Installation

1. Clone the repository
   ```bash
   git clone https://github.com/yourusername/HireSphere.git
   cd HireSphere
   ```

2. Restore dependencies
   ```bash
   dotnet restore
   ```

3. Build the project
   ```bash
   dotnet build
   ```

4. Run tests
   ```bash
   dotnet test
   ```

5. Run the application
   ```bash
   cd api/HireSphere.Application
   dotnet run
   ```

## 🧪 Testing

The project uses xUnit for unit testing with comprehensive test coverage:
- Model validation tests
- Business logic tests
- Repository contract tests

Run tests using:
```bash
dotnet test
```

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

## 📞 Contact

Your Name - [your.email@example.com](mailto:your.email@example.com)

Project Link: [https://github.com/yourusername/HireSphere](https://github.com/yourusername/HireSphere)

---

**Note**: This project is a work in progress. Contributions, suggestions, and feedback are welcome! 🌈
