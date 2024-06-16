# Usa una imagen base adecuada para tu aplicación
FROM python:3.9-slim

# Establece el directorio de trabajo
WORKDIR /app

# Copia el archivo requirements.txt al directorio de trabajo
COPY requirements.txt /app/

# Instala las dependencias necesarias
RUN pip install -r requirements.txt

# Copia los archivos generados y otros archivos necesarios
COPY file_pb2.py file_pb2_grpc.py /app/
COPY fileServer.py /app/

# Expone el puerto necesario para el servicio gRPC (si es necesario)
EXPOSE 50051

# Define environment variable
ENV NAME World
ENV HOST localhost
ENV PORT 50051

# Comando para ejecutar tu servidor gRPC
CMD ["python", "fileServer.py"]
