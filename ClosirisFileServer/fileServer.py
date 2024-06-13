import grpc
from concurrent import futures
from dotenv import load_dotenv
import os
import file_pb2
import file_pb2_grpc
from urllib.parse import unquote

class FileTransferServicer(file_pb2_grpc.FileTransferServicer):
    def UploadFile(self, request, context):
        user_id = None
        folder_name = None
        for key, value in context.invocation_metadata():
            if key == 'user_id':
                user_id = value
            elif key == 'folder_name':
                folder_name = value

        if user_id is None or folder_name is None:
            context.set_code(grpc.StatusCode.INVALID_ARGUMENT)
            context.set_details("Falta el ID de usuario o el nombre de la carpeta")
            return file_pb2.UploadResponse(message="Falta el ID de usuario o el nombre de la carpeta")

        filename = request.filename
        data = request.data

        print('User ID:', user_id)
        print('Folder Name:', folder_name)

        user_dir = os.path.join("./ClosirisFiles", str(user_id))
        os.makedirs(user_dir, exist_ok=True)

        file_dir = os.path.join(user_dir, folder_name)

        if os.path.exists(file_dir):
            print(f'La carpeta "{folder_name}" ya existe, se añadirá el archivo al directorio existente.')
        else:
            os.makedirs(file_dir)

        file_path = os.path.join(file_dir, filename)
        with open(file_path, "wb") as f:
            f.write(data)

        response = file_pb2.UploadResponse()
        response.message = "Archivo recibido correctamente"
        return response
    

    
    


    def DownloadFile(self, request, context):
        location_file = None
        for key, value in context.invocation_metadata():
            if key == 'location_file':
                location_file = unquote(value)

        if location_file is None:
            context.set_code(grpc.StatusCode.INVALID_ARGUMENT)
            context.set_details("Falta la ubicación del archivo")
            return file_pb2.UploadResponse(message="Falta la ubicación del archivo")

        print(f"Decodificado location_file: {location_file}")

        file_path = os.path.join("./", location_file)
        print(f"Ruta completa del archivo: {file_path}")

        if not os.path.exists(file_path):
            print("El archivo no existe en la ruta especificada.")
            context.set_code(grpc.StatusCode.NOT_FOUND)
            context.set_details("El archivo no existe")
            return file_pb2.DownloadResponse()

        with open(file_path, "rb") as f:
            data = f.read()

        response = file_pb2.DownloadResponse(data=data)
        return response
    
    
    def DeleteFile(self, request, context):
        fileLocation = request.fileLocation
        file_path = os.path.join("./", fileLocation)

        # Mensaje de depuración para la existencia del archivo
        print(f"Depuración: Verificando existencia del archivo en {file_path}")
        if not os.path.exists(file_path):
            context.set_code(grpc.StatusCode.NOT_FOUND)
            context.set_details("El archivo no existe")
            print("Depuración: El archivo no existe")
            return file_pb2.DeleteResponse()

        try:
            os.remove(file_path)
            print(f"Depuración: Archivo eliminado en {file_path}")
        except OSError as e:
            context.set_code(grpc.StatusCode.INTERNAL)
            context.set_details(f"No se pudo eliminar el archivo: {str(e)}")
            print(f"Depuración: No se pudo eliminar el archivo debido a: {str(e)}")
            return file_pb2.DeleteResponse(message=f"No se pudo eliminar el archivo: {str(e)}")

        # Obtener la ruta de la subcarpeta y la carpeta principal
        user_subfolder = os.path.dirname(file_path)
        user_folder = os.path.dirname(user_subfolder)
        
        print(f"Depuración: user_subfolder={user_subfolder}, user_folder={user_folder}")

        # Verificar si la subcarpeta está vacía y eliminarla si es así
        if os.path.exists(user_subfolder) and not os.listdir(user_subfolder):
            os.rmdir(user_subfolder)
            print(f"Depuración: Subcarpeta eliminada en {user_subfolder}")

            # Verificar si la carpeta del usuario está vacía después de eliminar la subcarpeta
            if os.path.exists(user_folder):
                subfolders = [f.path for f in os.scandir(user_folder) if f.is_dir()]
                print(f"Depuración: Subcarpetas en {user_folder}: {subfolders}")

                if not subfolders:  # Si no hay subcarpetas restantes
                    os.rmdir(user_folder)
                    print(f"Depuración: Carpeta del usuario eliminada en {user_folder}")
                elif len(subfolders) == 1 and not os.listdir(subfolders[0]):  # Si hay una única subcarpeta y está vacía
                    os.rmdir(subfolders[0])
                    os.rmdir(user_folder)
                    print(f"Depuración: Carpeta del usuario y única subcarpeta eliminadas en {user_folder}")

        response = file_pb2.DeleteResponse()
        response.message = "Archivo eliminado correctamente"
        print("Depuración: Archivo eliminado correctamente y respuestas enviadas")
        return response
    
load_dotenv()
    
def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    file_pb2_grpc.add_FileTransferServicer_to_server(FileTransferServicer(), server)
    
    host = os.getenv('HOST')
    port = os.getenv('PORT')
    
    server.add_insecure_port(f'{host}:{port}')
    server.start()
    print("Servidor gRPC escuchando en el puerto 50051")
    server.wait_for_termination()

if __name__ == '__main__':
    serve()