FROM node:22-alpine AS dependencias
WORKDIR /app/server
COPY server/package*.json ./
RUN npm ci --omit=dev

FROM node:22-alpine AS pruebas
WORKDIR /app/server
COPY server/package*.json ./
RUN npm ci
COPY protos /app/protos
COPY server ./
CMD ["npm", "run", "test:integracion"]

FROM node:22-alpine AS produccion
ENV NODE_ENV=production
WORKDIR /app/server
COPY --from=dependencias /app/server/node_modules ./node_modules
COPY protos /app/protos
COPY server/package.json ./
COPY server/src ./src
RUN mkdir -p /app/server/multimedia && chown -R node:node /app/server/multimedia
USER node
EXPOSE 3000 50051
HEALTHCHECK --interval=15s --timeout=5s --start-period=60s --retries=5 \
  CMD wget -qO- http://127.0.0.1:3000/salud || exit 1
CMD ["node", "src/index.js"]
