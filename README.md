# WriterID Backend Portal

A comprehensive ASP.NET Core Web API for managing writer identification tasks, datasets, models, and machine learning workflows. This backend provides RESTful APIs for both user-facing operations and external service integrations.

## 🚀 Features

### Core Functionality
- **User Authentication & Authorization** - JWT-based authentication with role-based access
- **Dataset Management** - Upload, analyze, and manage handwriting datasets
- **Model Training** - Train custom writer identification models
- **Task Execution** - Create and execute writer identification tasks
- **Dashboard Analytics** - User statistics and progress tracking
- **External API Integration** - API key authentication for external services

### Technical Features
- **Clean Architecture** - Layered architecture with separation of concerns
- **Repository Pattern** - Generic repository with Unit of Work pattern
- **AutoMapper** - Object-to-object mapping
- **Entity Framework Core** - MySQL database with migrations
- **Azure Storage Integration** - Blob storage for files and SAS URLs
- **Azure Queue Service** - Asynchronous message processing
- **Swagger Documentation** - Interactive API documentation

## 📁 Project Structure

```
WriterID.Dev.Portal/
├── Controllers/           # API endpoints
│   ├── AuthController.cs
│   ├── DashboardController.cs
│   ├── DatasetsController.cs
│   ├── ModelsController.cs
│   ├── TasksController.cs
│   └── External/         # External service APIs
│       ├── DatasetExternalController.cs
│       ├── ModelExternalController.cs
│       └── TaskExternalController.cs
├── Authentication/        # Custom authentication handlers
├── Program.cs            # Application startup
└── appsettings.json      # Configuration

WriterID.Dev.Portal.Model/
├── DTOs/                 # Data Transfer Objects
│   ├── Dashboard/
│   ├── Dataset/
│   ├── Model/
│   ├── Task/
│   ├── User/
│   └── External/
├── Entities/             # Domain entities
├── Configuration/        # App configuration models
└── Queue/               # Message queue models

WriterID.Dev.Portal.Service/
├── Services/             # Business logic services
├── Interfaces/           # Service contracts
└── Mappings/            # AutoMapper profiles

WriterID.Dev.Portal.Data/
├── Repositories/         # Data access layer
├── Interfaces/           # Repository contracts
├── Migrations/           # Database migrations
└── ApplicationDbContext.cs

WriterID.Dev.Portal.Core/
└── Enums/               # Shared enums
```

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: MySQL 8.0
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer + Custom API Key
- **Storage**: Azure Blob Storage
- **Messaging**: Azure Queue Storage
- **Documentation**: Swagger/OpenAPI
- **Mapping**: AutoMapper
- **Validation**: Data Annotations

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- MySQL 8.0 Server
- Azure Storage Account (for file storage)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd WriterID-Backend
   ```

2. **Configure Database**
   - Install MySQL 8.0
   - Create database: `writerid`
   - Update connection string in `appsettings.json`

3. **Configure Azure Storage**
   - Create Azure Storage Account
   - Update connection string in `appsettings.json`

4. **Install Dependencies**
   ```bash
   dotnet restore
   ```

5. **Run Migrations**
   ```bash
   dotnet ef database update
   ```

6. **Run the Application**
   ```bash
   dotnet run --project WriterID.Dev.Portal
   ```

### Configuration

Update `appsettings.json` with your settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;port=3306;Database=writerid;User=root;Password=your-password;",
    "AzureStorage": "your-azure-storage-connection-string"
  },
  "Jwt": {
    "Key": "your-jwt-secret-key",
    "Issuer": "auth",
    "Audience": "user"
  },
  "Authentication": {
    "ApiKey": "your-api-key-for-external-services"
  }
}
```

## 📚 API Documentation

### Authentication

#### User Authentication (JWT)
```http
POST /api/v1/auth/login
POST /api/v1/auth/register
```

#### External Service Authentication (API Key)
```http
X-API-Key: your-api-key
```

### Core Endpoints

#### Datasets
```http
GET    /api/v1/datasets              # Get user datasets
POST   /api/v1/datasets              # Create dataset
GET    /api/v1/datasets/{id}         # Get dataset details
PUT    /api/v1/datasets/{id}         # Update dataset
DELETE /api/v1/datasets/{id}         # Delete dataset
POST   /api/v1/datasets/{id}/analyze # Start dataset analysis
GET    /api/v1/datasets/{id}/analysis-results # Get analysis results
```

#### Models
```http
GET    /api/v1/models                 # Get user models
POST   /api/v1/models                 # Create model
GET    /api/v1/models/{id}            # Get model details
DELETE /api/v1/models/{id}            # Delete model
GET    /api/v1/models/{id}/training-results # Get training results
```

#### Tasks
```http
GET    /api/v1/tasks                  # Get user tasks
POST   /api/v1/tasks                  # Create task
GET    /api/v1/tasks/{id}             # Get task details
DELETE /api/v1/tasks/{id}             # Delete task
POST   /api/v1/tasks/{id}/execute     # Start task execution
GET    /api/v1/tasks/{id}/prediction  # Get prediction results
```

#### Dashboard
```http
GET    /api/v1/dashboard/stats        # Get user statistics
```

### External Service Endpoints

#### Dataset External API
```http
PUT    /api/external/datasets/status  # Update dataset status
GET    /api/external/datasets/{id}/status # Get dataset status
```

#### Model External API
```http
PUT    /api/external/models/status    # Update model status
GET    /api/external/models/{id}/status # Get model status
```

#### Task External API
```http
GET    /api/external/tasks/{id}/execution-info # Get task execution info
PUT    /api/external/tasks/status     # Update task status
GET    /api/external/tasks/{id}/status # Get task status
```

## 🔐 Security

### Authentication Methods

1. **JWT Bearer Token** - For user authentication
   - Used for all user-facing operations
   - Token includes user claims and roles

2. **API Key Authentication** - For external services
   - Used for external service integrations
   - Separate authentication scheme

### Authorization

- **User-based access** - Users can only access their own resources
- **Role-based permissions** - Future implementation for admin roles
- **API key scoping** - External services have limited access

## 🗄️ Database Schema

### Core Entities

- **User** - Application users with authentication
- **Dataset** - Handwriting datasets for training
- **Model** - Trained writer identification models
- **WriterIdentificationTask** - Tasks for writer identification

### Key Relationships

- Users own Datasets, Models, and Tasks
- Models are trained on Datasets
- Tasks use Models for prediction
- All entities have audit fields (CreatedAt, UpdatedAt)

## 🔄 Message Queue System

### Queue Messages

- **DatasetAnalysisMessage** - For dataset analysis processing
- **ModelTrainingMessage** - For model training processing
- **WriterIdentificationTaskMessage** - For task execution

### Processing Flow

1. User creates task/dataset/model
2. Backend queues message to Azure Storage Queue
3. External processor picks up message
4. Results are stored in Azure Blob Storage
5. Backend updates status via external API

## 🧪 Testing

### Running Tests
```bash
dotnet test
```

### API Testing
- Swagger UI available at `/swagger` in development
- Postman collection available in `/docs` folder

## 🚀 Deployment

### Development
```bash
dotnet run --project WriterID.Dev.Portal
```

### Production
```bash
dotnet publish -c Release
```

### Docker (Future)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY bin/Release/net8.0/publish/ App/
ENTRYPOINT ["dotnet", "App/WriterID.Dev.Portal.dll"]
```

## 📊 Monitoring & Logging

- **Structured Logging** - Using Microsoft.Extensions.Logging
- **Performance Monitoring** - Built-in ASP.NET Core metrics
- **Error Tracking** - Exception handling with detailed logging

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is part of the WriterID Final Year Project at IIT.

## 📞 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the API documentation at `/swagger`

---

**Last Updated**: January 2025  
**Version**: 1.0.0  
**Developed by**: WriterID Development Team 