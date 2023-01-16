#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.100-bullseye-slim AS build

# copy csproj and restore as distinct layers
COPY ["/src/Application/ReconNess.Application/ReconNess.Application.csproj", "/src/Application/ReconNess.Application/"]
COPY ["/src/Application/ReconNess.Application.Services/ReconNess.Application.Services.csproj", "/src/Application/ReconNess.Application.Services/"]
COPY ["/src/Domain/ReconNess.Domain/ReconNess.Domain.csproj", "/src/Domain/ReconNess.Domain/"]
COPY ["/src/Infrastructure/ReconNess.Infrastructure/ReconNess.Infrastructure.csproj", "/src/Infrastructure/ReconNess.Infrastructure/"]
COPY ["/src/Infrastructure/ReconNess.Infrastructure.DataAccess/ReconNess.Infrastructure.DataAccess.csproj", "/src/Infrastructure/ReconNess.Infrastructure.DataAccess/"]
COPY ["/src/Infrastructure/ReconNess.Infrastructure.Identity/ReconNess.Infrastructure.Identity.csproj", "/src/Infrastructure/ReconNess.Infrastructure.Identity/"]
COPY ["/src/Presentation/ReconNess.Presentation.Api/ReconNess.Presentation.Api.csproj", "/src/Presentation/ReconNess.Presentation.Api/"]
RUN dotnet restore "/src/Presentation/ReconNess.Presentation.Api/ReconNess.Presentation.Api.csproj"

# copy everything else and build app
COPY . .
WORKDIR /src/Presentation/ReconNess.Presentation.Api
RUN dotnet build "ReconNess.Presentation.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReconNess.Presentation.Api.csproj" -c Release -o /app/publish

FROM base AS final
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

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Presentation/ReconNess.Presentation.Api.dll"]