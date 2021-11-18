FROM mcr.microsoft.com/dotnet/sdk:6.0.100-bullseye-slim AS build
WORKDIR /app

RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs
RUN npm install -g @vue/cli

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ["DAL/ReconNess.Data.Npgsql/ReconNess.Data.Npgsql.csproj", "DAL/ReconNess.Data.Npgsql/"]
COPY ["ReconNess.Web/ReconNess.Web.csproj", "ReconNess.Web/"]
COPY ["ReconNess.Entities/ReconNess.Entities.csproj", "ReconNess.Entities/"]
COPY ["ReconNess.Core/ReconNess.Core.csproj", "ReconNess.Core/"]
COPY ["ReconNess.Worker/ReconNess.Worker.csproj", "ReconNess.Worker/"]
COPY ["ReconNess/ReconNess.csproj", "ReconNess/"]
RUN dotnet restore "ReconNess.Web/ReconNess.Web.csproj"

# copy everything else and build app
COPY . ./
WORKDIR /app/ReconNess.Web
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

# -------- Agents dependencies -------- 

# -------- End Agents dependencies -------- 

ENTRYPOINT ["dotnet", "ReconNess.Web.dll"]