# Simple Azure Deployment Script
param(
    [string]$ResourceGroup = "rg-smarttesttask",
    [string]$Location = "westeurope"
)

Write-Host "Starting Azure Deployment..." -ForegroundColor Green

# Login to Azure
az login

# Create Resource Group
az group create --name $ResourceGroup --location $Location

# Create App Service Plan
$planName = "plan-smarttesttask"
az appservice plan create `
    --name $planName `
    --resource-group $ResourceGroup `
    --sku B1 `
    --is-linux

# Create Web App
$appName = "app-smarttesttask-$(Get-Random -Maximum 9999)"
az webapp create `
    --name $appName `
    --resource-group $ResourceGroup `
    --plan $planName `
    --runtime "DOTNET:8.0"

# Create SQL Server
$sqlServer = "sql-smarttesttask-$(Get-Random -Maximum 9999)"
$sqlPassword = "Pass@word123!$(Get-Random -Maximum 999)"
az sql server create `
    --name $sqlServer `
    --resource-group $ResourceGroup `
    --admin-user sqladmin `
    --admin-password $sqlPassword

# Create SQL Database
az sql db create `
    --name SmartTestTaskDb `
    --server $sqlServer `
    --resource-group $ResourceGroup `
    --edition Basic

# Configure firewall
az sql server firewall-rule create `
    --resource-group $ResourceGroup `
    --server $sqlServer `
    --name AllowAzure `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0

# Build and publish app
Write-Host "Building application..." -ForegroundColor Yellow
cd src\SmartTestTask.API
dotnet publish -c Release -o ./publish

# Deploy to Azure
Write-Host "Deploying to Azure..." -ForegroundColor Yellow
Compress-Archive -Path ./publish/* -DestinationPath ./deploy.zip -Force
az webapp deployment source config-zip `
    --resource-group $ResourceGroup `
    --name $appName `
    --src ./deploy.zip

# Configure app settings
$connString = "Server=$sqlServer.database.windows.net;Database=SmartTestTaskDb;User Id=sqladmin;Password=$sqlPassword;Encrypt=true;"
$apiKey = "API_KEY_$(Get-Random -Maximum 999999)"

az webapp config appsettings set `
    --name $appName `
    --resource-group $ResourceGroup `
    --settings `
        ConnectionStrings__DefaultConnection="$connString" `
        ApiKey="$apiKey"

Write-Host "`nDeployment Complete!" -ForegroundColor Green
Write-Host "App URL: https://$appName.azurewebsites.net" -ForegroundColor Cyan
Write-Host "API Key: $apiKey" -ForegroundColor Yellow
Write-Host "SQL Password: $sqlPassword" -ForegroundColor Yellow
