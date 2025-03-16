public interface IOtpService
{
    Task<string> SendOtp(CreateOtpDto dto);
    Task<string> ValidateOtpAsync(ValidateOtpDto dto);

    Task<string> SendOtpWAAsync(CreateOtpDto dto);
    Task<string> ValidateOtpWAAsync(ValidateOtpDto dto);
}