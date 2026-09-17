# syntax=docker/dockerfile:1
FROM node:22-alpine AS build
WORKDIR /app
COPY FrontWeb/package.json FrontWeb/package-lock.json ./
RUN npm ci
COPY FrontWeb/ ./
RUN npm run build

FROM nginx:alpine AS final
COPY FrontWeb/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/FrontWeb/browser /usr/share/nginx/html
EXPOSE 80
