# Usa Node 20 buster-slim como la imagen base
FROM node:20-buster-slim

# Establece el directorio de trabajo
WORKDIR /app

# Instala el compilador de protos para Node.js
RUN npm install grpc-tools@latest -g

# Copia el archivo proto al directorio de trabajo del contenedor
COPY protos/file.proto /app/protos/file.proto

# Crea el directorio generado
RUN mkdir -p /app/generated

# Genera los stubs de gRPC
RUN grpc_tools_node_protoc \
    --proto_path=/app/protos \
    --js_out=import_style=commonjs,binary:/app/generated \
    --grpc_out=/app/generated \
    --plugin=protoc-gen-grpc=`which grpc_tools_node_protoc_plugin` \
    /app/protos/file.proto

# Copia los archivos package.json y package-lock.json al directorio de trabajo
COPY package.json package-lock.json ./

# Instala las dependencias
RUN npm install

# Copia todos los archivos de la aplicación al directorio de trabajo
COPY . .

# Exponer el puerto necesario para tu aplicación Node.js (si es necesario)
EXPOSE 3002

# Comando para iniciar la aplicación
CMD ["node", "app.js"]
