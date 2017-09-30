using log4net;
using log4net.Config;
using System;
using System.Linq;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace SqlToFlatFileLib.Logging
{
    /// <summary>
    /// A log4net logger.
    /// </summary>
    /// <seealso cref="ILogger" />
    public class Log4NetLogger : IAppLogger
    {
        /// <summary>
        /// The log4net logger
        /// </summary>
        private readonly ILog logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        public Log4NetLogger(string loggerName)
        {
            logger = LogManager.GetLogger(loggerName);
            XmlConfigurator.Configure();
            var hierarchy = LogManager.GetRepository() as Hierarchy;

            if (hierarchy != null)
            {
                var smtpAppender = (SmtpAppender)hierarchy.GetAppenders().Where(a => a.Name.Equals("SmtpAppender")).FirstOrDefault();

                if (smtpAppender != null)
                {
                    smtpAppender.Subject = smtpAppender.Subject.Replace("{machineName}", System.Environment.MachineName);
                }
            }

        }

        #region Debug

        /// <summary>
        /// Logs the specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            logger.Debug(message);
        }

        /// <summary>
        /// Logs the specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(string message, Exception exception)
        {
            logger.Debug(message, exception);
        }

        #endregion Debug

        #region Info

        /// <summary>
        /// Logs the specified info message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            logger.Info(message);
        }

        /// <summary>
        /// Logs the specified info message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(string message, Exception exception)
        {
            logger.Info(message, exception);
        }


        #endregion Info

        #region Warn

        /// <summary>
        /// Logs the specified warn message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            logger.Warn(message);
        }

        /// <summary>
        /// Logs the specified warn message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(string message, Exception exception)
        {
            logger.Warn(message, exception);
        }

        #endregion Warn

        #region Error

        /// <summary>
        /// Logs the specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            logger.Error(message);
        }

        /// <summary>
        /// Logs the specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(string message, Exception exception)
        {
            logger.Error(message, exception);
        }


        #endregion Error

        #region Fatal

        /// <summary>
        /// Logs the specified fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            logger.Fatal(message);
        }

        /// <summary>
        /// Logs the specified fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(string message, Exception exception)
        {
            logger.Fatal(message, exception);
        }

        #endregion Fatal
    }
}
