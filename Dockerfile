# Базовый образ для runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем только файлы проектов для восстановления зависимостей
COPY ["AccountService.csproj", "."]
COPY ["Directory.Build.props", "."]
RUN dotnet restore "AccountService.csproj"

# Копируем все остальные файлы
COPY . .

# Сборка проекта
RUN dotnet build "AccountService.csproj" -c Release -o /app/build

# Этап публикации
FROM build AS publish
RUN dotnet publish "AccountService.csproj" -c Release -o /app/publish \
    --no-restore \
    --no-build \
    -p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app

# Настройка пользователя (не-root для безопасности)
ARG UID=1000
RUN groupadd -g ${UID} appgroup && \
    useradd -u ${UID} -g appgroup -d /app -s /bin/sh --no-create-home appuser && \
    chown -R appuser:appgroup /app
USER appuser

# Копируем опубликованные файлы с сохранением прав
COPY --from=publish --chown=appuser:appgroup /app/publish .

# Оптимизация для контейнера
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_URLS=http://+:80 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
    DOTNET_NOLOGO=true \
    DOTNET_EnableDiagnostics=0

# Настройка healthcheck
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "AccountService.dll"]