
public interface IAuthService
{
    Task<Object> LoginAsync(LoginDto model);
    Task<Object> LoginGoogleAsync(string model);

    Task<Object> RegisterAsync(RegisterDto model);
    Task<Object> RegisterGoogleAsync(string model,RegisterGoogleDto login);

    Task<Object> Aktifasi(string id);
    Task<Object> UpdatePassword(string id, ChangeUserPasswordDto model);
    Task<string> ForgotPasswordAsync(UpdateUserAuthDto model);
    Task<Object> UpdatePin(string id, UpdatePinDto model);
    Task<Object> VerifyOtp(OtpDto otp);
    Task<Object> VerifyPin(string id);
    Task<Object> CheckPin(PinDto pin, string id);
    Task<string> CheckMail(string email);
    Task<Object> CheckStatus(string uid);

    Task<Object> RequestOtpEmail(string id);

}