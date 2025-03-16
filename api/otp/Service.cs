using MongoDB.Driver;
using SendingEmail;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace RepositoryPattern.Services.OtpService
{
    public class OtpService : IOtpService
    {
        private readonly IMongoCollection<OtpModel> _otpCollection;
        private readonly IEmailService _mailerService;

        public OtpService(IConfiguration configuration, IEmailService mailerService)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("trasgo");
            _otpCollection = database.GetCollection<OtpModel>("Otps");
            _mailerService = mailerService;
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

        public async Task<string> ValidateOtpWAAsync(ValidateOtpDto dto)
        {
            // Cari OTP berdasarkan email
            var otp = await _otpCollection.Find(o => o.Phone == dto.phonenumber).FirstOrDefaultAsync();

            if (otp == null)
                throw new CustomException(400, "Message", "OTP not found");

            if (otp.CodeOtp != dto.Code)
                throw new CustomException(400, "Message", "OTP invalid");

            // Hapus OTP setelah validasi
            await _otpCollection.DeleteOneAsync(o => o.Id == otp.Id);
            return "OTP valid";
        }

        Task<string> IOtpService.SendOtp(CreateOtpDto dto)
        {
            var accountSid = "AC0a38d8205f1771feba27c37c70d55bdc";
            var authToken = "9eb7770f0eb5867bcba1c6a869cd7892";
            TwilioClient.Init(accountSid, authToken);

            var verification = VerificationResource.Create(
                to: dto.Phonenumber,
                channel: "sms",
                pathServiceSid: "VAc8f6417c94c3de9696a25585185ec116"
            );

            Console.WriteLine(verification.Sid);
            return Task.FromResult("OTP sent to your email");
        }

        public async Task<string> ValidateOtpAsync(ValidateOtpDto dto)
        {
            var accountSid = "AC0a38d8205f1771feba27c37c70d55bdc";
            var authToken = "9eb7770f0eb5867bcba1c6a869cd7892";
            TwilioClient.Init(accountSid, authToken);

            var verificationCheck = VerificationCheckResource.Create(
                to: dto.phonenumber,
                code: dto.Code,
                pathServiceSid: "VAc8f6417c94c3de9696a25585185ec116"
            );

            Console.WriteLine(verificationCheck.Sid);
            return "Berhasil";
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
