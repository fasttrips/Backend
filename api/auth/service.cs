

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
        private readonly IMongoCollection<DriverAvalibleModel> dataDriverUser;

        private readonly IEmailService _emailService;
        private readonly string key;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConfiguration configuration, IEmailService emailService, ILogger<AuthService> logger)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("trasgo");
            dataUser = database.GetCollection<User>("User");
            dataDriverUser = database.GetCollection<DriverAvalibleModel>("DriverListAvailable");
            dataOtp = database.GetCollection<OtpModel>("OTP");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
            _emailService = emailService;
            _logger = logger;
        }

        // public async Task<object> RegisterGoogleAsync([FromBody] string data, RegisterGoogleDto login)
        // {
        //     try
        //     {
        //         var payload = JsonSerializer.Deserialize<JwtPayloads>(data);
        //         var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        //         var jwtService = new JwtService(configuration);

        //         var user = await dataUser.Find(u => u.Email == payload.Email).FirstOrDefaultAsync();

        //         if (user != null)
        //         {
        //             string tokens = jwtService.GenerateJwtToken(user.Id.ToString());
        //             return new { code = 200, id = user.Id.ToString(), accessToken = tokens };
        //         }
        //         var uuid = Guid.NewGuid().ToString();

        //         var roleData = new User()
        //         {
        //             Id = uuid,
        //             FullName = payload.Name,
        //             Email = payload.Email,
        //             Phone = "",
        //             Fcm = "",
        //             Balance = 0,
        //             Point = 0,
        //             IsActive = true,
        //             IsVerification = true,
        //             IdRole = Roles.User,
        //             CreatedAt = DateTime.Now,
        //         };

        //         await dataUser.InsertOneAsync(roleData);

        //         string token = jwtService.GenerateJwtToken(roleData.Id);
        //         return new { code = 200, id = roleData.Id, accessToken = token };
        //     }
        //     catch (CustomException ex)
        //     {

        //         throw new CustomException(400, "Error", ex.Message); ;
        //     }
        // }



        // public async Task<object> RequestOtpEmail(string id)
        // {
        //     try
        //     {

        //         var roleData = await dataUser.Find(x => x.Email == id).FirstOrDefaultAsync();
        //         if (roleData == null)
        //         {
        //             throw new CustomException(400, "Email", "Data not found");
        //         }
        //         Random random = new Random();
        //         string otp = random.Next(10000, 99999).ToString();
        //         var emailForm = new EmailForm()
        //         {
        //             Id = roleData.Id,
        //             Email = roleData.Email,
        //             Subject = "Request OTP",
        //             Message = "OTP",
        //             Otp = otp
        //         };
        //         var sending = _emailService.SendEmailAsync(emailForm);
        //         await dataUser.ReplaceOneAsync(x => x.Email == id, roleData);
        //         return new { code = 200, Message = "Berhasil" };
        //     }
        //     catch (CustomException ex)
        //     {

        //         throw;
        //     }
        // }

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

        public async Task<Object> CheckPin(string id, PinDto pin)
        {
            try
            {
                var user = await dataUser.Find(u => u.Phone == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Email", "Email tidak ditemukan");
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

        public async Task<object> UpdateProfile(string id, UpdateProfileDto item)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Phone == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
                roleData.FullName = item.FullName;
                roleData.Email = item.Email;
                await dataUser.ReplaceOneAsync(x => x.Phone == id, roleData);
                return new { code = 200, Message = "Update Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> UpdateUserProfile(string id, UpdateFCMProfileDto item)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Phone == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
                roleData.Fcm = item.FCM;
                await dataUser.ReplaceOneAsync(x => x.Phone == id, roleData);
                return new { code = 200, Message = "Update Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> UpdateDriverProfile(string id, DriverAvalibleModelDTO item)
        {
            try
            {
                var roleData = await dataDriverUser.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
                roleData.FCM = item.FCM;
                await dataDriverUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, Message = "Update Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> UpdateDriverLocationProfile(string id, DriverAvalibleModelDTO item)
        {
            try
            {
                var roleData = await dataDriverUser.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
                roleData.Latitude = item.Latitude;
                roleData.Longitude = item.Longitude;
                roleData.FCM = item.FCM;
                roleData.LastActive = item.LastActive;
                await dataDriverUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, Message = "Update Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> UpdateDriverStatusProfile(string id, DriverStatusServe item)
        {
            try
            {
                var roleData = await dataDriverUser.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
                roleData.IsStandby = item.IsStandby;
                await dataDriverUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, Message = "Update Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> GetDriverStatusProfile(string id)
        {
            try
            {
                var roleData = await dataDriverUser.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
                var itemdriver = await dataUser.Find(x=> x.Phone == roleData.Id).FirstOrDefaultAsync();
                if( itemdriver.Balance < 0 )
                {
                    roleData.IsStandby = false;
                    dataDriverUser.ReplaceOne(x => x.Id == id, roleData);
                    return new { code = 400, status = roleData, data = itemdriver };
                }
                return new { code = 200, status = roleData, data = itemdriver };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> SendNotif(PayloadNotifSend item)
        {
            try
            {
                string ServerKey = "AIzaSyDUKDzEbHSUxeI_d0Y8pAkEi8SydSz-TvQ";
                using var client = new HttpClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={ServerKey}");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var payload = new
                {
                    to = item.FCM,
                    notification = new
                    {
                        title = item.Title,
                        body = item.Body
                    },
                    data = new
                    {
                        forceOpen = "true" // Bisa digunakan untuk membuka app otomatis di Android
                    }
                };
                string json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://fcm.googleapis.com/fcm/send", content);
                string responseString = await response.Content.ReadAsStringAsync();
                return new { code = 200, Message = "Notification sent successfully", Response = responseString };
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
                var roleData = await dataUser.Find(x => x.Phone == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data not found");
                var user = new ModelViewUser
                {
                    Phone = roleData.Phone,
                    FullName = roleData.FullName,
                    Balance = roleData.Balance,
                    Point = roleData.Point,
                    Fcm = roleData.Fcm,
                    Image = roleData.Image,
                    Email = roleData.Email,
                };
                return new { code = 200, Id = roleData.Id, Data = user };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }
    }

    public class ModelViewUser
    {
        public string? Id { get; set; }
        public string? Phone { get; set; }
        public string? FullName { get; set; }
        public float? Balance { get; set; }
        public float? Point { get; set; }
        public string? Fcm { get; set; }
        public string? Image { get; set; }
        public string? Email { get; set; }

    }
}