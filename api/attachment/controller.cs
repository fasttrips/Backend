
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Trasgo.Server.Controllers
{
    [ApiController]
    [Route("api/v1/file")]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _IAttachmentService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        private readonly IMongoCollection<MediaFile> AttachmentLink;
        private readonly ConvertJWT _ConvertJwt;
        private readonly IConfiguration _conf;
        public AttachmentController(IConfiguration configuration, IAttachmentService roleService, ConvertJWT convert)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Trasgo");
            AttachmentLink = database.GetCollection<MediaFile>("MediaFile");
            _IAttachmentService = roleService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
            _ConvertJwt = convert;
            _conf = configuration;
        }

        [HttpGet]
        [Route("review/{fileId}")]
        public async Task<IActionResult> Preview(string fileId)
        {
            try
            {
                var client = new MongoClient(_conf.GetConnectionString("ConnectionURI"));
                var database = client.GetDatabase("Trasgo");
                var gridFSBucket = new GridFSBucket(database);

                // Convert fileId to ObjectId
                var objectId = new ObjectId(fileId);

                // Download file from GridFS
                var stream = await gridFSBucket.OpenDownloadStreamAsync(objectId);

                // Get metadata
                var fileName = stream.FileInfo.Filename;
                var contentType = stream.FileInfo.Metadata.GetValue("ContentType", "").AsString;

                // Set content disposition to inline for preview in browser
                Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");

                // Return the file stream directly for preview
                return File(stream, contentType);
            }
            catch (GridFSFileNotFoundException)
            {
                return NotFound(new { status = false, message = "File not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "An error occurred", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [RequestSizeLimit(300 * 1024 * 1024)] // 300 MB
        [Route("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new CustomException(400, "Message", "File not found");
                }

                // Batasi ukuran file
                const long maxFileSize = 300 * 1024 * 1024; // 300 MB
                if (file.Length > maxFileSize)
                {
                    throw new CustomException(400, "Message", "File size must not exceed 300 MB.");
                }

                // Mengecilkan ukuran gambar jika file adalah gambar
                byte[] compressedImage;
                using (var image = Image.Load(file.OpenReadStream()))
                {
                    // Mengurangi kualitas tetapi tetap menjaga resolusi
                    var encoder = new JpegEncoder
                    {
                        Quality = 20 // Sesuaikan dengan target ukuran (0-100)
                    };

                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, encoder);
                        compressedImage = ms.ToArray();
                    }
                }

                // Simpan ke GridFS
                var client = new MongoClient(_conf.GetConnectionString("ConnectionURI"));
                var database = client.GetDatabase("Trasgo");
                var gridFSBucket = new GridFSBucket(database);

                ObjectId fileId;
                using (var stream = new MemoryStream(compressedImage))
                {
                    var options = new GridFSUploadOptions
                    {
                        Metadata = new BsonDocument
                {
                    { "FileName", file.FileName },
                    { "ContentType", "image/jpeg" },
                    { "UploadedBy", "admin" },
                    { "UploadedAt", DateTime.UtcNow }
                }
                    };

                    fileId = await gridFSBucket.UploadFromStreamAsync(file.FileName, stream, options);
                }

                return Ok(new
                {
                    status = true,
                    message = "File uploaded successfully",
                    fileId = fileId.ToString(),
                    path = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/v1/Attachment/Download/{fileId}"
                });
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "An error occurred", details = ex.Message });
            }
        }
    }
}

public class MediaFile
{
    public string? Id { get; set; } // Unique file ID
    public string? FileName { get; set; } // Original file name
    public string? ContentType { get; set; } // MIME type
    public long FileSize { get; set; } // File size in bytes
    public string? UploadedBy { get; set; } // User ID who uploaded the file
    public byte[]? Data { get; set; } // File data as byte array
    public DateTime UploadedAt { get; set; } // Upload timestamp
}
