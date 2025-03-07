

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CheckId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SendingEmail;
using Trasgo.Shared.Models;

namespace RepositoryPattern.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<User> dataUser;
        private readonly IMongoCollection<OtpModel> dataOtp;

        private readonly IEmailService _emailService;
        private readonly string key;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConfiguration configuration, IEmailService emailService, ILogger<AuthService> logger)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("trasgo");
            dataUser = database.GetCollection<User>("Users");
            dataOtp = database.GetCollection<OtpModel>("Otps");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<object> RegisterGoogleAsync([FromBody] string data, RegisterGoogleDto login)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<JwtPayloads>(data);
                var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                var jwtService = new JwtService(configuration);

                var user = await dataUser.Find(u => u.Email == payload.Email).FirstOrDefaultAsync();

                if (user != null)
                {
                    string tokens = jwtService.GenerateJwtToken(user.Id.ToString());
                    return new { code = 200, id = user.Id.ToString(), accessToken = tokens };
                }
                var uuid = Guid.NewGuid().ToString();

                var roleData = new User()
                {
                    Id = uuid,
                    FullName = payload.Name,
                    Email = payload.Email,
                    Phone = "",
                    Fcm= "",
                    Balance=0,
                    Point=0,
                    IsActive = true,
                    IsVerification = true,
                    IdRole = Roles.User,
                    CreatedAt = DateTime.Now,
                };

                await dataUser.InsertOneAsync(roleData);

                string token = jwtService.GenerateJwtToken(roleData.Id);
                return new { code = 200, id = roleData.Id, accessToken = token };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message);;
            }
        }

        public async Task<object> UpdatePin(string id, UpdatePinDto item)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Error", "Data tidak ada");
                }
                if (item.Pin.Length < 6)
                {
                    throw new CustomException(400, "Password", "Pin harus 6 karakter");
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(item.Pin);
                roleData.Pin = hashedPassword;
                await dataUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, Message = "Update Pin Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> RequestOtpEmail(string id)
        {
            try
            {

                var roleData = await dataUser.Find(x => x.Email == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Email", "Data not found");
                }
                Random random = new Random();
                string otp = random.Next(10000, 99999).ToString();
                var emailForm = new EmailForm()
                {
                    Id = roleData.Id,
                    Email = roleData.Email,
                    Subject = "Request OTP",
                    Message = "OTP",
                    Otp = otp
                };
                var sending = _emailService.SendEmailAsync(emailForm);
                roleData.Otp = otp;
                await dataUser.ReplaceOneAsync(x => x.Email == id, roleData);
                return new { code = 200, Message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw;
            }
        }

        public async Task<object> VerifyOtp(OtpDto otp)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Email == otp.Email).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Error", "Data not found");
                }
                if (roleData.Otp != otp.Otp)
                {
                    throw new CustomException(400, "Error", "Otp anda salah");
                }
                var data = new LoginDto();
                {
                    data.Email = roleData.Email;
                }
                var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                var jwtService = new JwtService(configuration);
                string userId = roleData.Id;
                string token = jwtService.GenerateJwtToken(userId);
                return new { code = 200, message = "Berhasil", accessToken = token };
            }
            catch (CustomException ex)
            {

                throw;
            }
        }


        public async Task<object> Aktifasi(string id)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data not found");
                return new { code = 200, Id = roleData.Id, Data = roleData };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> VerifyPin(string id)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Message", "Data Not Found");
                }
                if (roleData.Pin == null)
                {
                    return new CustomException(400, "Message", "Pin Not Set");
                }
                return new { code = 200, Message = "Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<Object> CheckPin([FromBody] PinDto pin, string id)
        {
            try
            {
                var user = await dataUser.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new CustomException(400, "Email", "Email tidak ditemukan");
                }
                bool isPinCorrect = BCrypt.Net.BCrypt.Verify(pin.Pin, user.Pin);
                if (!isPinCorrect)
                {
                    throw new CustomException(400, "Pin", "Pin Salah");
                }
                string idAsString = user.Id.ToString();
                return new { code = 200, Message = "Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<string> CheckMail(string email)
        {
            try
            {
                var userOtp = await dataUser.Find(x => x.Email == email).FirstOrDefaultAsync();
                if (userOtp == null)
                {
                    return "Email Not Found";
                }
                throw new CustomException(400,"Message", "Already Registered");
            }
            catch (Exception ex)
            {

                throw new CustomException(400,"Message", $"{ex.Message}");
            }
        }
    }
}