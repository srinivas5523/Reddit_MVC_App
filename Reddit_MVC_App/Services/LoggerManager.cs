using NLog;
using System;
namespace Reddit_MVC_App.Services
{
    public class LoggerManager : ILoggerManager
    {
        Logger log = LogManager.GetLogger("ErrorLogFile");

        public void LogError(string ErrorMessage)
        {
            log.Error(ErrorMessage);
        }
        public void LogError(Exception ex, string ErrorMessage)
        {
            log.Error(ex, ErrorMessage);
        }
        public void LogInfo(string ErrorMessage)
        {
            log.Info(ErrorMessage);
        }
        public void LogDebug(string ErrorMessage)
        {
            log.Debug(ErrorMessage);
        }
        public void LogTrace(string ErrorMessage)
        {
            log.Trace(ErrorMessage);
        }
    }

}
