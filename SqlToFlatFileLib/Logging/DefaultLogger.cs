using System;

namespace SqlToFlatFileLib.Logging
{
    /// <summary>
    /// Provide a threadsafe default logger for components that don't want to pass a logger instance through the code.
    /// For components that do want to provide different loggers, this instance can also be used as desired.
    /// Singleton implementation based on MSDN:
    /// https://msdn.microsoft.com/en-us/library/ff650316.aspx
    /// </summary>
    public sealed class DefaultLogger
    {
        /// <summary>
        /// The private singleton instance.
        /// </summary>
        private static volatile IAppLogger instance;

        /// <summary>
        /// Used to make the singleton threadsafe.
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// Prevents a default instance of the <see cref="DefaultLogger"/> class from being created.
        /// </summary>
        private DefaultLogger() { }

        /// <summary>
        /// Gets the singleton logging instance.
        /// </summary>
        public static IAppLogger Instance
        { 
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Log4NetLogger("Default Logger");
                        }
                    }
                }

                return instance;
            }
        }
    }
}
