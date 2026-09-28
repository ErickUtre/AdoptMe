FROM nginx:1.27-alpine

COPY default.conf /etc/nginx/conf.d/default.conf

HEALTHCHECK --interval=15s --timeout=5s --retries=5 CMD wget -qO- http://127.0.0.1/salud || exit 1
