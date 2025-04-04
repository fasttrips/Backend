public interface IChatService
{

    Task<object> SendChatWAAsync(CreateChatDto dto);
    Task<object> GetChatWAAsync(GetChatDto dto);

}