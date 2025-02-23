using MongoDB.Driver;
using Trasgo.Shared.Models;

namespace RepositoryPattern.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly string bucketName = "Trasgo";
        private readonly IMongoCollection<Attachments> AttachmentLink;
        private readonly IMongoCollection<User> users;

        private readonly string key;

        public AttachmentService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Trasgo");
            AttachmentLink = database.GetCollection<Attachments>("Attachment");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }
        public async Task<Object> Get(string Username)
        {
            try
            {
                var items = await AttachmentLink.Find(_ => _.UserId == Username).ToListAsync();
                return new { code = 200, data = items, message = "Complete" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        // public async Task<(string FileName, string Url)> Upload(IFormFile file, string fileName, string idUser)
        // {
        //     var bucket = bucketName;
        //     var folderName = "uploads";
        //     var fileSize = file.Length;

        //     // Baca file sebagai stream
        //     using var memoryStream = new MemoryStream();
        //     await file.CopyToAsync(memoryStream);
        //     memoryStream.Position = 0;

        //     // Tentukan metadata file
        //     var contentType = file.ContentType;

        //     // Unggah file ke Google Cloud Storage
        //     var gcsFile = storageClient.UploadObject(bucket, fileName, contentType, memoryStream);

        //     // URL akses file
        //     var url = $"https://storage.googleapis.com/{bucket}/{fileName}";
        //     var uuid = Guid.NewGuid().ToString();

        //     var otp = new Attachments
        //     {
        //         Id = uuid,
        //         fileName = fileName,
        //         type = file.ContentType,
        //         path = url,
        //         UserId = idUser,
        //         size = fileSize,
        //         CreatedAt = DateTime.Now,
        //     };

        //     // Simpan OTP ke database
        //     await AttachmentLink.InsertOneAsync(otp);

        //     return (gcsFile.Name, url);
        // }
    }
}