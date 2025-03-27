using MongoDB.Driver;
using Trasgo.Shared.Models;

namespace RepositoryPattern.Services.OtpService
{
    public class OtpService : IOtpService
    {
        private readonly IMongoCollection<OtpModel> _otpCollection;
        private readonly IMongoCollection<User> _userCollection;

        public OtpService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("trasgo");
            _otpCollection = database.GetCollection<OtpModel>("OTP");
            _userCollection = database.GetCollection<User>("User");
        }

        public async Task<string> SendOtpWAAsync(CreateOtpDto dto)
        {
            // Hapus OTP yang sudah ada sebelumnya
            var existingOtps = await _otpCollection.Find(otp => otp.Phone == dto.Phonenumber).ToListAsync();
            foreach (var otps in existingOtps)
            {
                await _otpCollection.DeleteOneAsync(o => o.Id == otps.Id);
            }

            // Generate OTP baru
            var otpCode = new Random().Next(1000, 9999).ToString();
            var otp = new OtpModel
            {
                Phone = dto.Phonenumber,
                CodeOtp = otpCode,
                TypeOtp = dto.Phonenumber,
                CreatedAt = DateTime.UtcNow
            };

            // Simpan OTP ke database
            await _otpCollection.InsertOneAsync(otp);
            var emailBody = $"Your OTP code is: {otpCode}";
            // Kirim email
            try
            {
                var emailForm = new EmailForm()
                {
                    Phone = dto.Phonenumber,
                    Subject = "Request OTP",
                    Message = "OTP",
                    Otp = otpCode
                };
                return "OTP sent to your wa " + otpCode;
            }
            catch (Exception)
            {
                throw new CustomException(400, "Message", "Failed to send OTP email");
            }
        }

        public async Task<object> ValidateOtpWAAsync(ValidateOtpDto dto)
        {
            // Cari OTP berdasarkan email
            var otp = await _otpCollection.Find(o => o.Phone == dto.phonenumber).FirstOrDefaultAsync();

            if (otp == null)
                throw new CustomException(400, "Message", "OTP not found");

            if (otp.CodeOtp != dto.Code)
                throw new CustomException(400, "Message", "OTP invalid");

            var users = await _userCollection.Find(o => o.Phone == dto.phonenumber).FirstOrDefaultAsync();
            var uuid = Guid.NewGuid().ToString();
            if(users == null)
            {
                var userModel = new User
                {
                    Id = uuid,
                    FullName = "Pengguna " + dto.phonenumber,
                    Phone = dto.phonenumber,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Email = "",
                    Image = "",
                    IdRole = "67e4a5739b655dbba418982d",
                    Pin = "",
                    Balance = 0,
                    Point = 0,
                    Fcm = "",
                    IsActive = true,
                    IsVerification = true
                };
                await _userCollection.InsertOneAsync(userModel);
            }

            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var jwtService = new JwtService(configuration);
            // Hapus OTP setelah validasi
            string token = jwtService.GenerateJwtToken(dto.phonenumber);
            await _otpCollection.DeleteOneAsync(o => o.Id == otp.Id);
            return new { code = 200, accessToken = token };
        }

        public class EmailForm
        {
            public string? Id { get; set; }
            public string? Phone { get; set; }
            public string? Subject { get; set; }
            public string? Message { get; set; }
            public string? Otp { get; set; }
        }


    }
}
