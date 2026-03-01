# SmartMentor - Career Guidance API

## 📋 Project Overview

**SmartMentor** is a comprehensive career guidance platform built with ASP.NET Core. It helps users identify their skills, interests, and career goals, while providing personalized career path recommendations and gap analysis based on their requirements.

### Key Features
- **User Authentication & Authorization** - Secure JWT-based authentication
- **Skill Management** - Track user skills with proficiency levels
- **Interest Tracking** - Record user interests for career matching
- **Career Goals** - Define and track career aspirations
- **Gap Analysis** - Identify skill gaps for target career paths
- **User Profile Completion** - Structured onboarding process
- **Role-Based Access Control** - Admin, Mentor, and User roles

---

## 🏗️ Project Architecture

### Layered Architecture Pattern

```
SmartMentorApi (Presentation Layer)
    ↓
SmartMentor.Application (Business Logic)
    ↓
SmartMentor.Abstraction (Contracts & DTOs)
    ↓
SmartMentor.Domain (Entities & Rules)
    ↓
SmartMentor.Persistence (Data Access & EF Core)
```

### Project Structure

#### **1. SmartMentor.Domain** 📦
**Core business entities and rules**
- **Entities/** - Core domain models
  - `ApplicationUser` - Extended identity user
  - `Skill`, `UserSkills` - Skill management
  - `Interests`, `UserInterests` - Interest tracking
  - `CareerGoal`, `CareerGoalRequiredSkill` - Career path management
  - `Jwt` - JWT token entity

- **Identity/** - ASP.NET Core Identity integration
  - `ApplicationUser` - Custom user entity
  - `ApplicationRole` - Custom role entity

- **EntityConfigurations/** - EF Core Fluent API configurations
  - Relationship mappings
  - Constraints and indexes
  - Data type specifications

- **Enums/** - Enumeration types
  - `SkillLevelEnum` - Junior, Intermediate, Senior, Expert

#### **2. SmartMentor.Abstraction** 📄
**Contracts (Interfaces) and Data Transfer Objects (DTOs)**
- **Services/** - Service interfaces
  - `IAuthenticationService` - Login, register, token validation
  - `ICompleteUserProfileService` - Profile setup operations
  - `IGapAnalysisService` - Career gap analysis

- **Repositories/** - Data access interfaces
  - `IGenericRepository<T>` - Generic CRUD operations
  - `IUnitOfWork` - Transaction management

- **Dto/** - Data transfer objects
  - **Requests/** - API request models
  - **Responses/** - API response models
  - **SharedRequestsAndResponses/** - Common DTOs

#### **3. SmartMentor.Application** 🔧
**Business logic implementations**
- **Implementations/** - Service implementations
  - `AuthenticationService` - User authentication logic
  - `CompleteUserProfileService` - Profile completion workflow
  - `GapAnalysisService` - Analysis algorithms

#### **4. SmartMentor.Persistence** 💾
**Data access and database configuration**
- **Data/** - Database context and seeders
  - `ApplicationDbContext` - EF Core DbContext
  - `DataSeeder` - Initial data setup
  - `CareerGoalRequiredSkillSeeder` - Career data population
  - `UserSkillsInterestsSeeder` - Sample user data

- **Migrations/** - EF Core database migrations
  - Track schema changes
  - Enable database versioning

- **Repositories/** - Repository implementations

#### **5. SmartMentorApi** 🚀
**ASP.NET Core API (Presentation Layer)**
- **Program.cs** - Configuration and dependency injection setup
- **DiContainer.cs** - Dependency injection container configuration
- **Controllers/** - API endpoints
- **Extentions/** - Extension methods and custom middleware
- **appsettings.json** - Configuration (production)
- **appsettings.Development.json** - Configuration (development)
- **SmartMentorApi.http** - REST client file for testing

---

## 🛠️ Technology Stack

| Layer | Technology |
|-------|----------|
| **API Framework** | ASP.NET Core 6.0+ |
| **Database** | SQL Server / PostgreSQL |
| **ORM** | Entity Framework Core |
| **Authentication** | ASP.NET Core Identity + JWT |
| **API Documentation** | Swagger/OpenAPI |
| **Dependency Injection** | Built-in DI Container |
| **Language** | C# 10+ |

---

## 📋 Prerequisites

Before running the project, ensure you have:

- **.NET SDK** - Version 6.0 or higher
  - Download: [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
  - Verify: `dotnet --version`

- **SQL Server or PostgreSQL** - Latest stable version
  - SQL Server: [Download](https://www.microsoft.com/sql-server/sql-server-downloads)
  - PostgreSQL: [Download](https://www.postgresql.org/download/)

- **Visual Studio** (or VS Code)
  - VS 2022+ recommended
  - OR VS Code with C# extension

- **Git** - Version control
  - Download: [git-scm.com](https://git-scm.com/)

---

## ⚙️ Installation & Setup

### Step 1: Clone the Repository
```powershell
git clone <repository-url>
cd SmartMentorApi
```

### Step 2: Restore Dependencies
```powershell
dotnet restore
```

### Step 3: Configure Database Connection
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartMentorDb;Trusted_Connection=true;Encrypt=false;"
  }
}
```

**For PostgreSQL:**
```json
"DefaultConnection": "Host=localhost;Database=smartmentordb;Username=postgres;Password=your_password"
```

### Step 4: Apply Database Migrations
```powershell
cd SmartMentorApi
dotnet ef database update --project ../SmartMentor.Persistence
```

### Step 5: Run the Application
```powershell
dotnet run
```

The API will be available at: `https://localhost:7000` (or port shown in console)

---

## 🔑 Database Setup

### Create Database from Migrations
```powershell
# Apply all pending migrations
dotnet ef database update --project ../SmartMentor.Persistence

# Revert to previous migration
dotnet ef database update <migration-name> --project ../SmartMentor.Persistence
```

### Seed Initial Data
Data seeders run automatically on application startup:
- `DataSeeder` - Core setup data
- `CareerGoalRequiredSkillSeeder` - Career path templates
- `UserSkillsInterestsSeeder` - Sample user data

### Create New Migration
```powershell
dotnet ef migrations add <MigrationName> --project ../SmartMentor.Persistence
```

---

## 🚀 Running the Application

### Development Mode
```powershell
dotnet run --project SmartMentorApi
```

### Production Mode
```powershell
dotnet run --project SmartMentorApi --configuration Release
```

### Using Visual Studio
1. Open `SmartMentorApi.sln`
2. Set `SmartMentorApi` as startup project
3. Press `F5` or click **Start Debugging**

### Using VS Code
1. Press `Ctrl+F5` to run without debugging
2. Press `F5` to run with debugging

---

## 📡 API Endpoints Overview

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/refresh-token` | Refresh JWT token |
| POST | `/api/auth/logout` | User logout |

### User Profile
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/user/profile` | Get user profile |
| PUT | `/api/user/profile` | Update profile |
| POST | `/api/user/complete-profile` | Complete registration |

### Skills
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/skills` | List all skills |
| POST | `/api/user-skills` | Add skill to user |
| DELETE | `/api/user-skills/{id}` | Remove skill |

### Interests
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/interests` | List all interests |
| POST | `/api/user-interests` | Add interest |
| DELETE | `/api/user-interests/{id}` | Remove interest |

### Career Analysis
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/gap-analysis` | Analyze skill gaps |
| GET | `/api/career-goals` | Get career paths |

---

## 🧪 Testing

### Using SmartMentorApi.http
The `.http` file contains pre-configured requests for testing:
```
@baseUrl = https://localhost:7000

### Register
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

### Using Swagger UI
After running the application, navigate to:
```
https://localhost:7000/swagger
```

### Using Postman
1. Import the API endpoints
2. Create a collection for each feature
3. Set environment variables for `baseUrl` and `token`

---

## 📝 Documentation Guidelines

### How to Document in README

#### **1. Code Comments**
```csharp
/// <summary>
/// Analyzes user skills against career goals
/// </summary>
/// <param name="userId">The user identifier</param>
/// <returns>Gap analysis result</returns>
public async Task<GapAnalysisDto> AnalyzeGaps(int userId)
{
    // Retrieve user's current skills
    var userSkills = await _userSkillsRepository.GetByUserIdAsync(userId);
    
    // ... implementation
}
```

#### **2. Documentation Structure**
```markdown
## Feature Name

### Description
Brief explanation of functionality

### Components Involved
- Entity: `SkillEntity`
- Service: `ISkillService`
- DTO: `SkillDto`
- Controller: `SkillsController`

### Usage Example
Code example here

### Database Schema
Entity relationships diagram
```

#### **3. Updating README for New Features**

When adding a new feature:

1. **Update Project Structure Section**
   ```markdown
   - **NewService/** - Description of what it does
     - `Interface` - What it defines
     - `Implementation` - How it works
   ```

2. **Add API Endpoints Table**
   ```markdown
   | Method | Endpoint | Description |
   |--------|----------|-------------|
   | GET | `/api/feature` | Get feature data |
   ```

3. **Document Configuration**
   ```json
   // Add to appsettings.json
   "FeatureSettings": {
     "Enabled": true,
     "MaxItems": 100
   }
   ```

4. **Add Setup Steps** (if database changes)
   ```powershell
   dotnet ef migrations add AddNewFeature
   dotnet ef database update
   ```

#### **4. Entity Documentation Template**
```csharp
/// <summary>
/// Represents a user skill with proficiency level
/// </summary>
public class UserSkill : BaseEntity
{
    /// <summary>
    /// Foreign key to user
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Proficiency level (Junior, Intermediate, Senior, Expert)
    /// </summary>
    public SkillLevel Level { get; set; }
}
```

#### **5. Service Documentation Template**
```csharp
/// <summary>
/// Handles skill-related business logic
/// </summary>
public interface ISkillService
{
    /// <summary>
    /// Retrieves all skills
    /// </summary>
    /// <returns>List of skills</returns>
    Task<IEnumerable<SkillDto>> GetAllSkillsAsync();
}
```

#### **6. API Endpoint Documentation**
```csharp
/// <summary>
/// Get all skills available in the system
/// </summary>
/// <returns>List of all skills</returns>
/// <response code="200">Returns the list of skills</response>
[HttpGet]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<ActionResult<IEnumerable<SkillDto>>> GetSkills()
{
    // Implementation
}
```

---

## 📂 Naming Conventions

Follow these conventions for consistency:

| Type | Convention | Example |
|------|-----------|---------|
| **Classes** | PascalCase | `UserSkillService` |
| **Methods** | PascalCase + Verb | `GetUserSkillsAsync()` |
| **Variables** | camelCase | `userSkills`, `isActive` |
| **Constants** | UPPER_SNAKE_CASE | `MAX_SKILL_LEVEL` |
| **Interfaces** | I + PascalCase | `ISkillService` |
| **DTOs** | EntityName + Dto | `SkillDto`, `UserresponseDto` |
| **Enums** | PascalCase + Enum | `SkillLevelEnum` |
| **Database Tables** | Plural + PascalCase | `UserSkills`, `CareerGoals` |

---

## 🔄 Development Workflow

### Creating a New Feature

1. **Create Entity** (SmartMentor.Domain)
   ```csharp
   public class NewEntity : BaseEntity { }
   ```

2. **Create Configuration** (SmartMentor.Domain/EntityConfigurations)
   ```csharp
   public class NewEntityConfiguration : IEntityTypeConfiguration<NewEntity> { }
   ```

3. **Create DTOs** (SmartMentor.Abstraction/Dto)
   ```csharp
   public class NewEntityDto { }
   ```

4. **Create Service Interface** (SmartMentor.Abstraction/Services)
   ```csharp
   public interface INewService { }
   ```

5. **Create Service Implementation** (SmartMentor.Application/Implementations)
   ```csharp
   public class NewService : INewService { }
   ```

6. **Create Repository** (SmartMentor.Persistence)
   ```csharp
   public class NewRepository : IGenericRepository<NewEntity> { }
   ```

7. **Create Controller** (SmartMentorApi/Controllers)
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class NewController : ControllerBase { }
   ```

8. **Create Migration**
   ```powershell
   dotnet ef migrations add AddNewEntity
   ```

9. **Update Documentation** (README.md)

---

## 🐛 Troubleshooting

### Common Issues

#### **1. Database Connection Error**
```
"A network-related or instance-specific error occurred"
```
**Solution:**
- Verify SQL Server is running: `Services.msc`
- Check connection string in `appsettings.json`
- Ensure database exists: `CREATE DATABASE SmartMentorDb`

#### **2. Null Reference Exception in Service**
**Solution:**
- Verify dependency injection in `DiContainer.cs`
- Check if service is registered: `services.AddScoped<IService, Service>()`

#### **3. Migration Error**
```
"The model for context 'ApplicationDbContext' has changed since compilation"
```
**Solution:**
```powershell
dotnet ef remove migrations
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

#### **4. JWT Token Validation Failed**
**Solution:**
- Check token expiration in `appsettings.json`
- Verify secret key is same in config
- Ensure Authorization header format: `Bearer <token>`

---

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [JWT Authentication](https://jwt.io)
- [C# Coding Standards](https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style)

---

## 👥 Contributors

- **Developer** - [Your Name]
- **Project Lead** - [Mentor Name]

---

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 🤝 Support

For issues or questions:
1. Check existing documentation
2. Review troubleshooting section
3. Create a GitHub issue with:
   - Error message
   - Steps to reproduce
   - System information

---

**Last Updated:** February 2026
**Version:** 1.0.0
