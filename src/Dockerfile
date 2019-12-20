FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs
RUN npm install -g @vue/cli

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ["DAL/ReconNet.Data.Npgsql/ReconNet.Data.Npgsql.csproj", "DAL/ReconNet.Data.Npgsql/"]
COPY ["ReconNet.Web/ReconNet.Web.csproj", "ReconNet.Web/"]
COPY ["ReconNet.Entities/ReconNet.Entities.csproj", "ReconNet.Entities/"]
COPY ["ReconNet.Core/ReconNet.Core.csproj", "ReconNet.Core/"]
COPY ["ReconNet/ReconNet.csproj", "ReconNet/"]
RUN dotnet restore "ReconNet.Web/ReconNet.Web.csproj"

# copy everything else and build app
COPY . ./
WORKDIR /app/ReconNet.Web
RUN dotnet publish -c Release -o /dist

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000

COPY --from=build /dist ./

ENTRYPOINT ["dotnet", "ReconNet.Web.dll"]