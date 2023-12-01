# Use a lean container to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
# Set the timezone inside the container to Berlin timezone
ENV TZ=Europe/Berlin



# Set environment variables for German locale
ENV LANG=de_DE.UTF-8
ENV LC_ALL=de_DE.UTF-8
ENV LANGUAGE=de_DE.UTF-8


WORKDIR /app
# expose the ports used by the application
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5000


       

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser 

# Use a container that contains the complete sdk to build the container
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

WORKDIR /src
USER root
RUN apt-get update -yq \
    && apt-get upgrade -y \
    && apt-get -yq install curl gnupg ca-certificates \
    && curl -L https://deb.nodesource.com/setup_18.x | bash \
    && apt-get update -yq \
    && apt-get install nodejs  
RUN npm install -g npm@9.7.2
COPY ["NACTAM.csproj", "./"]
RUN dotnet restore "NACTAM.csproj"

COPY wwwroot/package.json .
COPY wwwroot/package-lock.json .
RUN npm ci
COPY . .

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
#Remove database file before running dotnet ef database update, because dotnet ef database update will create a fresh database.
RUN rm -f ./Persistence/NACTAM.db
RUN dotnet ef database update
RUN dotnet publish "NACTAM.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
LABEL com.centurylinklabs.watchtower.enable="False"
USER root
RUN chown -R appuser /app
USER appuser
ENTRYPOINT ["dotnet", "NACTAM.dll"]
