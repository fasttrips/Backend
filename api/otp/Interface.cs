public interface IOtpService
{

    Task<string> SendOtpWAAsync(CreateOtpDto dto);
    Task<string> ValidateOtpWAAsync(ValidateOtpDto dto);
}