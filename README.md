# RosterVideo

RosterVideo is a small Razor Pages application (targeting .NET 10) that collects simple roster entries in memory and exposes a JSON feed.

Run locally
1. Open the solution in Visual Studio 2026 and run (F5) using IIS Express or the project profile.
2. Or use the CLI:
   - dotnet restore
   - dotnet build
   - dotnet run --project RosterVideo/RosterVideo.csproj

JSON Feed
- The JSON feed is available at: https://localhost:5001/api/roster (adjust port/profile used). The endpoint returns application/json.

Notes
- Data is stored in-memory and will be lost when the process restarts.
- The project includes unit and integration tests under tests/RosterVideo.Tests. Run them with `dotnet test`.
- CI: A GitHub Actions workflow is included at .github/workflows/ci.yml to build and test on push/pull_request.

Deployment
- Publish to Azure App Service using Visual Studio publish profile or `dotnet publish` and follow Azure App Service deployment steps.

CI/CD & Azure Deployment
- The GitHub Actions workflow (.github/workflows/ci.yml) builds, tests, publishes, and deploys to Azure App Service on pushes to main/master.
- To enable deployment, set the following repository secrets:
  - AZURE_WEBAPP_NAME: the name of your Azure App Service
  - AZURE_WEBAPP_PUBLISH_PROFILE: the publish profile XML content (obtain from the Azure Portal -> Get publish profile)

After adding these secrets, pushes to main/master will deploy the published app automatically.
