FROM mcr.microsoft.com/mssql/server:2022-latest

COPY --chmod=755 inicializar.sh /usr/src/adoptme/inicializar.sh
COPY --chmod=644 esquema.sql /usr/src/adoptme/esquema.sql

HEALTHCHECK --interval=10s --timeout=5s --start-period=60s --retries=20 CMD test -f /tmp/adoptme-listo

ENTRYPOINT ["/usr/src/adoptme/inicializar.sh"]
