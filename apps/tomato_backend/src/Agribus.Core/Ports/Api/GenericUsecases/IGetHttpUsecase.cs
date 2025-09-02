namespace Agribus.Core.Ports.Api.GenericUsecases
{
    public interface IGetHttpUsecase
    {
        Task<string> GetAsync(string url, Dictionary<string, string>? parameters);
    }
}
