using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace SosoVisionTool.Tools
{
    public class SosoLogManager : ISosoLogManager
    {
        private readonly IEventAggregator _eventAggregator;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public SosoLogManager(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void ShowLogInfo(string msg)
        {
            LogPublish(msg, LogLevel.Info);

            logger.Info(msg);
        }

        private void LogPublish(string msg, LogLevel level)
        {
            string log = $"{DateTime.Now.ToString("yyyy-MM-dd hh:MM:ss")}  {msg}";
            LogStruct logStruct = new LogStruct(log, level);
            _eventAggregator.GetEvent<MessageEvent>().Publish(logStruct);
        }

        public void ShowLogWarning(string msg)
        {
            LogPublish(msg, LogLevel.Warning);

            logger.Error(msg);
        }

        public void ShowLogError(string msg)
        {
            LogPublish(msg, LogLevel.Error);
            logger.Error(msg);
        }
    }
}
