namespace Reddit_MVC_App.Services
{
    public interface ILoggerManager
    {
        void LogError(string ErrorMessage);
        void LogError(Exception ex, string ErrorMessage);
        void LogTrace(string ErrorMessage);
        void LogDebug(string ErrorMessage);
        void LogInfo(string ErrorMessage);
    }
}
