# SmartTestTask - Equipment Placement Contract Management System

## 📋 Project Description

SmartTestTask is a contract management system for placing equipment at production facilities.  
The application is built with **.NET 8**, using **Clean Architecture**, the **CQRS pattern**, and **Domain-Driven Design (DDD)**.

---

## 🏗️ Architecture

- **SmartTestTask.Domain** – domain layer (entities, interfaces, events)  
- **SmartTestTask.Application** – business logic (commands, queries, handlers)  
- **SmartTestTask.Infrastructure** – infrastructure (database, repositories, services)  
- **SmartTestTask.API** – Web API layer (controllers, middleware)  
- **SmartTestTask.Tests** – unit and integration tests  

---

## 🔧 Tech Stack

- .NET 8.0  
- ASP.NET Core Web API  
- Entity Framework Core 8  
- SQL Server  
- MediatR (CQRS)  
- FluentValidation  
- AutoMapper  
- Azure Service Bus (optional)  
- Docker  
- Swagger / OpenAPI  

---

## 📦 Prerequisites

### Local Development
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- [SQL Server 2019+](https://www.microsoft.com/sql-server) or [SQL Server LocalDB](https://docs.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb)  
- [Visual Studio 2022](https://visualstudio.microsoft.com/), [Visual Studio Code](https://code.visualstudio.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/)  
- [Git](https://git-scm.com/)  

### Azure Deployment
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)  
- Active Azure subscription  
- [PowerShell Core](https://github.com/PowerShell/PowerShell) (for deployment scripts)  

---

## 🚀 Local Run

### 1. Clone the repository
git clone https://github.com/your-username/SmartTestTask.git
cd SmartTestTask
2. Configure the database
Option A: LocalDB (recommended for development)
Verify LocalDB is installed:

bash
Копировать код
sqllocaldb info
Create LocalDB instance (if needed):
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB

Option B: Full SQL Server
Open src/SmartTestTask.API/appsettings.json

Update the connection string:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=SmartTestTaskDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}

3. Configure API Key
Open src/SmartTestTask.API/appsettings.Development.json

Set your key (default: DEV_API_KEY_12345):
{
  "ApiKey": "YOUR_SECURE_API_KEY_HERE"
}

4. Apply database migrations
cd src/SmartTestTask.API
dotnet ef database update

If EF tools are missing:
dotnet tool install --global dotnet-ef
dotnet ef database update

5. Run the application
bash
Копировать код
cd src/SmartTestTask.API
dotnet run
Or via IDE:

Open SmartTestTask.sln

Set SmartTestTask.API as Startup Project

Press F5 / Run

6. Verify
Open: https://localhost:7255 or http://localhost:5120

Swagger UI will be available

Use header:
X-API-Key: DEV_API_KEY_12345

🐳 Run with Docker
Option 1: Build & run manually
docker build -f src/SmartTestTask.API/Dockerfile -t smarttesttask-api .
docker run -d -p 8080:80 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=SmartTestTaskDb;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True" \
  -e ApiKey="YOUR_API_KEY" \
  --name smarttesttask \
  smarttesttask-api
Option 2: Using Docker Compose (recommended)
Since docker-compose.yml is already included in the repository:
docker-compose up -d

This will start both SQL Server and the API service.

☁️ Azure Deployment
Option 1: Automated deployment via PowerShell script (scripts/deploy-azure.ps1)

Option 2: Manual setup via Azure Portal

Option 3: Deployment via GitHub Actions CI/CD

(See full deployment guide in /scripts.)

📝 API Usage
Authentication
All endpoints (except /api/health) require:
X-API-Key: YOUR_API_KEY

Endpoints
Health Check
GET /api/health

Contracts
GET /api/contracts
GET /api/contracts/{id}
POST /api/contracts
PUT /api/contracts/{id}
DELETE /api/contracts/{id}
Example:

POST /api/contracts
Headers: X-API-Key: YOUR_API_KEY
Body:
{
  "productionFacilityCode": "FAC-001",
  "processEquipmentTypeCode": "EQT-001",
  "equipmentQuantity": 5
}
Facilities

GET /api/facilities
Equipment Types

GET /api/equipmenttypes
🧪 Testing
bash
Копировать код
dotnet test
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
🔍 Logging & Monitoring
Local logs: src/SmartTestTask.API/logs/

Azure: use Log Stream + Application Insights
