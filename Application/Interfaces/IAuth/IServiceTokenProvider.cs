namespace Application.Interfaces.IAuth
{
    public interface IServiceTokenProvider
    {
        Task<string> GetServiceTokenAsync();
    }
}


