# Docker build & run (LinguaTech API)

This file shows basic commands to build and run the LinguaTech API container on Windows PowerShell.

Build the API image (from repository root):

```powershell
# Build directly with Dockerfile (run from repository root next to LinguaTech.sln)
docker build -f Dockerfile -t linguatech-api:local .

# Or use docker-compose to build and run (recommended for dev with multiple services)
docker-compose build
```

Run the image:

```powershell
# Run single container
docker run --rm -p 5000:80 --name linguatech-api linguatech-api:local

# Or use docker-compose to start (reads docker-compose.yml in repo root)
docker-compose up -d

# View logs
docker-compose logs -f linguatech-api

# Stop
docker-compose down
```

Notes:

-   The Dockerfile targets .NET 8.0. Ensure your Docker host supports .NET 8 base images.
-   Add required environment variables (connection strings, secrets) in `docker-compose.yml` or provide an `.env` file and reference it.
-   If your application depends on a database (Postgres), add a service to `docker-compose.yml` and update the connection string to point to that service.
