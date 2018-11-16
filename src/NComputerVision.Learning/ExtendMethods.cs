using log4net;

namespace NComputerVision.Learning
{
    public static class ExtendMethods
    {
        public static void Info(this ILog log, string format, params object[] ar)
        {
            log.Info(string.Format(format, ar));
        }
    }
}
