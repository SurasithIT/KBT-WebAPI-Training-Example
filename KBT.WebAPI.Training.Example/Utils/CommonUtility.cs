using System;
using log4net;

namespace KBT.WebAPI.Training.Example.Utils
{
	public static class CommonUtility
	{
        public static void GetLoggerThreadId()
        {
            if (ThreadContext.Properties["threadid"] == null)
            {
                ThreadContext.Properties["threadid"] = Thread.CurrentThread.ManagedThreadId;
            }
        }
    }
}

