-- Создание пользователя
CREATE USER keycloak WITH PASSWORD 'password';

-- Создание базы данных
CREATE DATABASE keycloak OWNER keycloak;

-- Предоставление всех привилегий на базу данных
GRANT ALL PRIVILEGES ON DATABASE keycloak TO keycloak;

-- Подключение к базе keycloak для настройки схемы
\connect keycloak


-- Обязательное расширение для Keycloak
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Настройка прав для работы Keycloak
GRANT ALL ON SCHEMA public TO keycloak;
ALTER SCHEMA public OWNER TO keycloak;