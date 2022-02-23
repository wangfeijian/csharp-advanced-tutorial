using System.Windows.Media;

namespace SosoVisionCommonTool.Log
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
    public class LogStruct
    {
        public LogStruct(string message, LogLevel logLevel)
        {
            Message = message;
            LogLevel = logLevel;

            switch (LogLevel)
            {
                case LogLevel.Info:
                    BackColor = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                    break;
                case LogLevel.Warning:
                    BackColor = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    break;
                case LogLevel.Error:
                    BackColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    break;
            }
        }

        public string Message { get; set; }

        public LogLevel LogLevel { get; set; }

        public Brush BackColor { get; set; }
    }
}
