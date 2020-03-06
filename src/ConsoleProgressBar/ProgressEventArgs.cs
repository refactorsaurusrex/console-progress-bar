using System;
using JetBrains.Annotations;

namespace ConsoleProgressBar
{
    /// <inheritdoc />
    [PublicAPI]
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastDataReceived"></param>
        /// <param name="timeOutTriggered"></param>
        public ProgressEventArgs(DateTime lastDataReceived, DateTime timeOutTriggered)
        {
            LastDataReceived = lastDataReceived;
            TimeOutTriggered = timeOutTriggered;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastDataReceived { get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime TimeOutTriggered { get;  }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Elapsed => TimeOutTriggered - LastDataReceived;
    }
}
