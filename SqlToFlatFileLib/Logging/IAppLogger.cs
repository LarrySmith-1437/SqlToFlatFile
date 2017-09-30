using System;

namespace SqlToFlatFileLib.Logging
{
    public interface IAppLogger
    {
        #region Debug

        void Debug(string message);
        void Debug(string message, Exception exception);

        #endregion Debug

        #region Info

        void Info(string message);
        void Info(string message, Exception exception);

        #endregion Info

        #region Warn

        void Warn(string message);
        void Warn(string message, Exception exception);

        #endregion Warn

        #region Error

        void Error(string message);
        void Error(string message, Exception exception);

        #endregion Error

        #region Fatal

        void Fatal(string message);
        void Fatal(string message, Exception exception);

        #endregion Fatal
    }
}
