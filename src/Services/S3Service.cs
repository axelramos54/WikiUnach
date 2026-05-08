using System;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
namespace WUNACH
{
    internal static class S3Service
    {
        // Credenciales leidas desde App.config (ver Seccion 7)
        private static readonly string BucketName =
        System.Configuration.ConfigurationManager.AppSettings["AWS_BucketName"];
        private static readonly string Region =
        System.Configuration.ConfigurationManager.AppSettings["AWS_Region"];
        private static readonly string AccessKey =
        System.Configuration.ConfigurationManager.AppSettings["AWS_AccessKey"];
        private static readonly string SecretKey =
        System.Configuration.ConfigurationManager.AppSettings["AWS_SecretKey"];
        private static AmazonS3Client CrearCliente()
        {
            var creds = new Amazon.Runtime.BasicAWSCredentials(AccessKey, SecretKey);
            var config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.USEast2 };
            return new AmazonS3Client(creds, config);
        }
        // Sube un archivo local a S3 y devuelve la URL publica
        public static string SubirArchivo(string rutaLocal,
        string prefijo = "archivos")
        {
            string ext = Path.GetExtension(rutaLocal);
            string key = $"{prefijo}/{Guid.NewGuid()}{ext}";
            using (var cliente = CrearCliente())
            using (var tx = new TransferUtility(cliente))
            {
                tx.Upload(new TransferUtilityUploadRequest
                {
                    FilePath = rutaLocal,
                    BucketName = BucketName,
                    Key = key
                });
            }
            return $"https://{BucketName}.s3.{Region}.amazonaws.com/{key}";
        }
        // Genera una URL temporal valida por N minutos (para buckets privados)
        public static string ObtenerUrlDescarga(string key,
        int minutosExpiracion = 60)
        {
            using (var cliente = CrearCliente())
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = BucketName,
                    Key = key,
                    Expires = DateTime.UtcNow.AddMinutes(minutosExpiracion)
                };
                return cliente.GetPreSignedURL(request);
            }
        }
        // Elimina un archivo del bucket (ej. al borrar una página)
        public static void EliminarArchivo(string key)
        {
            using (var cliente = CrearCliente())
            {
                cliente.DeleteObject(new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = key
                });
            }
        }
    }
}
