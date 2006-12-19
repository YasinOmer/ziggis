using System;
using log4net;
using log4net.Core;

namespace ZigGis.Utilities
{
    public class CLogger : ILog
    {
        public ILog internalLog;

        public CLogger(Type t)
        {
            this.internalLog = log4net.LogManager.GetLogger(t);
        }

        public CLogger(String logName)
        {
            this.internalLog = log4net.LogManager.GetLogger(logName);
        }

        public void enterFunc(string funcName)
        {
            NDC.Push(funcName);
            if (internalLog.IsDebugEnabled) internalLog.Debug("[enter]");
        }

        public void leaveFunc()
        {
            if (internalLog.IsDebugEnabled) internalLog.Debug("[exit]");
            NDC.Pop();
        }

        #region ILog
        public void Debug(object message, Exception t)
        {
            internalLog.Debug(message, t);
        }

        public void Debug(object message)
        {
            internalLog.Debug(message);
        }

        public void Error(object message, Exception t)
        {
            internalLog.Error(message, t);
        }

        public void Error(object message)
        {
            internalLog.Error(message);
        }

        public void Fatal(object message, Exception t)
        {
            internalLog.Fatal(message, t);
        }

        public void Fatal(object message)
        {
            internalLog.Fatal(message);
        }

        public void Info(object message, Exception t)
        {
            internalLog.Info(message, t);
        }

        public void Info(object message)
        {
            internalLog.Info(message);
        }

        public bool IsDebugEnabled { get { return internalLog.IsDebugEnabled; } }

        public bool IsErrorEnabled { get { return internalLog.IsErrorEnabled; } }

        public bool IsFatalEnabled { get { return internalLog.IsFatalEnabled; } }

        public bool IsInfoEnabled { get { return internalLog.IsInfoEnabled; } }

        public bool IsWarnEnabled { get { return internalLog.IsWarnEnabled; } }

        public void Warn(object message, Exception t)
        {
            internalLog.Warn(message, t);
        }

        public void Warn(object message)
        {
            internalLog.Warn(message);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DebugFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DebugFormat(string format, object o1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DebugFormat(string format, object o1, object o2)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void DebugFormat(string format, object o1, object o2, object o3)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ErrorFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ErrorFormat(string s, object o1, object o2, object o3)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ErrorFormat(string s, object o1)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void ErrorFormat(string s, object o1, object o2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(string format, object o1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(string format, object o1, object o2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FatalFormat(string format, object o1, object o2, object o3)
        {
            throw new Exception("The method or operation is not implemented.");
        }


        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InfoFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InfoFormat(string format, object o1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InfoFormat(string format, object o1, object o2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InfoFormat(string format, object o1, object o2, object o3)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(string format, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(string format, object o1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(string format, object o1, object o2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WarnFormat(string format, object o1, object o2, object o3)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region ILoggerWrapper
        public ILogger Logger
        {
            get { return ((ILoggerWrapper)internalLog).Logger; }
        }
        #endregion
    }
}