namespace VotingSystem.Application.Services.Logging
{
    public interface ILoggingService
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
