FROM mcr.microsoft.com/dotnet/sdk:6.0.100-bullseye-slim AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ["src/Application/ReconNess.Application/ReconNess.Application.csproj", "Application/ReconNess.Application/"]
COPY ["src/Application/ReconNess.Application.Services/ReconNess.Application.Services.csproj", "Application/ReconNess.Application.Services/"]
COPY ["src/Domain/ReconNess.Domain/ReconNess.Domain.csproj", "Domain/ReconNess.Domain/"]
COPY ["src/Infrastructure/ReconNess.Infrastructure/ReconNess.Infrastructure.csproj", "/Infrastructure/ReconNess.Infrastructure/"]
COPY ["src/Infrastructure/ReconNess.Infrastructure.DataAccess/ReconNess.Infrastructure.DataAccess.csproj", "/Infrastructure/ReconNess.Infrastructure.DataAccess/"]
COPY ["src/Infrastructure/ReconNess.Infrastructure.Identity/ReconNess.Infrastructure.Identity.csproj", "/Infrastructure/ReconNess.Infrastructure.Identity/"]
COPY ["src/Presentation/ReconNess.Presentation.Api/ReconNess.Presentation.Api.csproj", "Presentation/ReconNess.Presentation.Api/"]
RUN dotnet restore "Presentation/ReconNess.Presentation.Api/ReconNess.Presentation.Api.csproj"

# copy everything else and build app
COPY . ./
WORKDIR /app/Presentation/ReconNess.Presentation.Api
RUN dotnet publish -c Release -o /dist

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

#####################################################################################################################
# If you want to generate your own certificate with different password
# you can run
#
# dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p { password here }
# dotnet dev-certs https --trust
# 
# and replace `reconness\src\aspnetapp.pfx` with the file `%USERPROFILE%\.aspnet\https\aspnetapp.pfx` generated
# and replace the password that you used `{ password here }` 
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password="{ password here }"
#####################################################################################################################

COPY aspnetapp.pfx .

ENV ASPNETCORE_URLS http://+:5000;https://+:5001
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="aspnetapp.pfx"
EXPOSE 5000
EXPOSE 5001

COPY --from=build /dist ./

ENTRYPOINT ["dotnet", "ReconNess.Presentation.Api.dll"]