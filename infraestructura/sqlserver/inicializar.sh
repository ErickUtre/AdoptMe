#!/bin/bash
set -euo pipefail

readonly MARCA_LISTO=/tmp/adoptme-listo
readonly ESQUEMA=/usr/src/adoptme/esquema.sql
readonly SQLCMD=(/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b)

rm -f "$MARCA_LISTO"
/opt/mssql/bin/sqlservr &
readonly PID_SERVIDOR=$!

until "${SQLCMD[@]}" -Q "SELECT 1" > /dev/null 2>&1; do
  echo "Esperando a que SQL Server acepte conexiones..."
  sleep 3
done

"${SQLCMD[@]}" -Q "IF SUSER_ID(N'$DB_USER') IS NULL CREATE LOGIN [$DB_USER] WITH PASSWORD = N'$DB_PASSWORD';"

preparar_base_datos() {
  local nombre="$1"
  "${SQLCMD[@]}" -Q "IF DB_ID(N'$nombre') IS NULL CREATE DATABASE [$nombre];"
  "${SQLCMD[@]}" -d "$nombre" -f 65001 -i "$ESQUEMA"
  "${SQLCMD[@]}" -d "$nombre" -Q "IF USER_ID(N'$DB_USER') IS NULL BEGIN CREATE USER [$DB_USER] FOR LOGIN [$DB_USER]; ALTER ROLE db_owner ADD MEMBER [$DB_USER]; END;"
  echo "Base de datos $nombre lista"
}

preparar_base_datos "$DB_NAME"
preparar_base_datos "$DB_TEST_NAME"

touch "$MARCA_LISTO"
wait "$PID_SERVIDOR"
