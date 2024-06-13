# Usa Node 20 buster-slim como la imagen base
FROM node:20-buster-slim

# Establece el directorio de trabajo
WORKDIR /app

# Copia los archivos package.json y package-lock.json al directorio de trabajo
COPY package.json package-lock.json ./

# Instala las dependencias
RUN npm install

# Copia todos los archivos de la aplicación al directorio de trabajo
COPY . .

# Exponer el puerto 3000
EXPOSE 3002

# Inicia la aplicación
CMD ["node", "app.js"]
